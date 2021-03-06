﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.Ingame.Move;
using TicTacTorus.Source.LobbySpecificContent;
using TicTacTorus.Source.Persistence;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.ServerHandler;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Hubs
{
    public class ConnectionHubServer : Hub
    {
        /// <summary>
        /// Adds a User to the connected User List in the Server
        /// </summary>
        /// <param name="user"></param>
        public void OnConnectedPlayer(HumanPlayer user = null)
        {
            Server.Instance.AddConnectedUser(Context.ConnectionId, user);
        }

        /// <summary>
        /// Removes a User from the connected User List in the Server
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Server.Instance.RemoveConnectedUser(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        #region Lobby
        public async Task CreateLobby(HumanPlayer hp)
        {
            var lobby = LobbyHandler.CreateLobby(hp);
            await Clients.Caller.SendAsync("ReceiveLobbyId", lobby.Id.ToString());
        }
        
        public async Task JoinLobby(string lobbyId, string player)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var hPlayer = JsonConvert.DeserializeObject<HumanPlayer>(player);

            // logic
            var lobby= LobbyHandler.AddPlayerToLobby(lobbyId, hPlayer);
            /*
            if (lobby == null)
            {
                await Clients.Caller.SendAsync("ReceiveLobbyExisting", false);
            }
            */
            // prepare json-strings
            var jsLobby = JsonConvert.SerializeObject(lobby, Formatting.Indented, settings);
            var jsPlayers = JsonConvert.SerializeObject(lobby.Players, Formatting.Indented, settings);

            // add to group and send
            await Clients.Caller.SendAsync("GetLobby", jsLobby, hPlayer.Index);
            await Clients.Group(lobbyId).SendAsync("PlayerListChanged", jsPlayers);
            await Groups.AddToGroupAsync(Context.ConnectionId,
                Game.UniquePlayerGroup(new Base64(lobbyId),
                    Server.Instance.Lobbies[lobbyId].Players.FindIndex(p => p.InGameName == hPlayer.InGameName)));
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
        }
        
        public async Task RemovePlayerFromLobby(string lobbyId, byte index)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            
            // logic
            var (playerList, removedPlayer) = LobbyHandler.RemovePlayerFromLobby(lobbyId, index);

            // notify removed player
            await Clients.Group(Game.UniquePlayerGroup(lobbyId, index)).SendAsync("LeaveLobby");
            
            //await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);
            await Clients.Group(lobbyId).SendAsync("ReceiveMessage", removedPlayer.InGameName, "I left the Lobby");
            await Clients.Group(lobbyId).SendAsync("PlayerListChanged", JsonConvert.SerializeObject(playerList, Formatting.Indented, settings));
        }
        /*
        public async Task LeaveLobby(string lobbyId, string player)
        {
            await RemovePlayerFromLobby(lobbyId, player);
        }*/

        public async Task CheckLobbyExisting(string id)
        {
            var exists = !Server.Instance.LobbyIdIsUnique(id);
            await Clients.Caller.SendAsync("ReceiveLobbyExisting", exists);
        }

        public async Task OnConnectedUser(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        #endregion
        #region LobbyContentChange

        public async Task ChangeLobbyDescription(string lobbyId, string descr)
        {
            var lobby = Server.Instance.GetLobbyById(lobbyId);

            if (lobby.Description != descr)
            {
                lobby.Description = descr;
                await Clients.Group(lobbyId).SendAsync("DescriptionChanged", lobby.Description );
            }
        }
        public async Task ChangeLobbyStatus(string lobbyId, string stat)
        {
            var lobby = Server.Instance.GetLobbyById(lobbyId);

            if (lobby.Status != stat)
            {
                lobby.Status = stat;
                await Clients.Group(lobbyId).SendAsync("StatusChanged", lobby.Status);
            }
        }
        
        public async Task ChangeLobbyName(string lobbyId, string title)
        {
            var lobby = Server.Instance.GetLobbyById(lobbyId);

            if (lobby.Name != title)
            {
                lobby.Name = title;
                await Clients.Group(lobbyId).SendAsync("TitleChanged", lobby.Name);
            }
        }

        /*
        public async Task ChangeLobbyPlayerCount(string lobbyId, int maxCount)
        {
            var lobby = Server.Instance.GetLobbyById(lobbyId);
            if (lobby.Players.Count > maxCount) // ? needed?
            {
                // not possible -> send message
                Clients.Caller.SendAsync("ReceiveMessage", "System",
                    "Can not reduce count of players. There are currently too many player in this Lobby.");
            }
            else
            {
                lobby.MaxPlayerCount = maxCount;
                Clients.Group(lobbyId).SendAsync("MaxPlayersChanged", maxCount);
            }
        }

        public async Task ChangeLobbyVisibility(string lobbyId, bool isPrivate)
        {
            Server.Instance.GetLobbyById(lobbyId).IsPrivate = isPrivate;
            // maybe send refresh to LobbyHandler (and every person on lobbylist-site)?
        }
        */

        #endregion
        #region LobbyToGame
        
        public async Task StartGame(string lobbyId)
        {
            GameHandler.CreateGame(lobbyId, Clients);
            await Clients.Group(lobbyId).SendAsync("ChangeToGame", true);
            //TODO Returns Game to Clients? (Jack need your confirmation. Check pls last commit if that's ok what I have done)
            //await Clients.Caller.SendAsync("", Server.Instance.CreateGameFromLobby(lobby));
        }

        public async Task ConnectToGame(string gameId, string jsPlayer)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var player = JsonConvert.DeserializeObject<HumanPlayer>(jsPlayer, settings);

            var response = GameHandler.AddPlayerToGame(gameId, player);
            if (response.Item2)
            {
                var jsGame = JsonConvert.SerializeObject(response.Item1, Formatting.Indented, settings);
                await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
                await Groups.AddToGroupAsync(Context.ConnectionId,
                    Game.UniquePlayerGroup(new Base64(gameId),
                        Server.Instance.ClientGames[gameId].players.FindIndex(p => p.InGameName == player?.InGameName)));
                //await Clients.Group(gameId).SendAsync("ReceiveGameInformation", jsGame);
                await Clients.Caller.SendAsync("ReceiveGameInformation", jsGame, (byte)response.Item3);
            }
            else
            {
                await Clients.Caller.SendAsync("ConnectToGameFailed", "game_has_started");
            }
        }

        #endregion
        #region GameInput

        public async Task ReceivePlacementMove(string gameId, string jsonMove)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var move = JsonConvert.DeserializeObject<PlacementMove>(jsonMove, settings);
            var (validMove, winnerMessage, nextPlayer) = GameHandler.PlaceMove(gameId, move);
            if (validMove)
            {
                // send everyone
                await Clients.Group(gameId).SendAsync("ReceivePlayerMove", jsonMove);
                await Clients.Group(Game.UniquePlayerGroup(gameId, nextPlayer)).SendAsync("YourTurn");
                if (winnerMessage != null)
                {
                    var message = winnerMessage.Split('!');
                    await Clients.Group(gameId).SendAsync("ReceiveAlert", message[0], message[1]);
                }
            }
            else
            {
                // send error
                Clients.Group(Game.UniquePlayerGroup(gameId, move.Owner)).SendAsync("ReceiveAlert", "Error", "Invalid Move");
            }
        }

        #endregion

        #region GameOutput

        public async void SendMoveError(string gameId, string message, byte indx)
        {
            await Clients.Group(gameId).SendAsync("ReceiveMessage", "Referee", message);
            await Clients.Group(gameId + indx).SendAsync("MoveFailed");
        }
        
        #endregion
        #region Chat
        
        public async Task SendMessage(string lobbyId, string user, string message)
        {
            await Clients.Group(lobbyId).SendAsync("ReceiveMessage", 
                Server.Instance.GetConnectedUser(Context.ConnectionId).InGameName, message);
        }
        
        public async Task SendMessage2(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage2", user, message);
        }
        
        #endregion
        #region Login / Register

        public async Task ConfirmLogin(string id, string pw)
        {
            //Check in Database
            try
            {
                var (player, validation) = PersistenceStorage.LoadPlayer(id, pw);
                if (validation)
                {
                    var playerJson = JsonConvert.SerializeObject(player);
                    await Clients.Client(Context.ConnectionId)
                        .SendAsync("LoginSuccess", playerJson);
                }
                else
                {
                    await Clients.Caller.SendAsync("LoginFailed", "Login failed. Wrong userID or Password! ");
                }
            }
            catch (SQLiteException e)
            {
                await Clients.Caller.SendAsync("LoginFailed", "Database Connection Error: " + e);
            }
        }
        public async Task ConfirmRegister(string u)
        {
            HumanPlayer user = JsonConvert.DeserializeObject<HumanPlayer>(u);
            try
            {
                if (PersistenceStorage.CheckPlayerIdIsUnique(user.ID))
                {
                    //If ID is unique
                    if (PersistenceStorage.CreatePlayer(user))
                    {
                        await Clients.Caller.SendAsync("RegisterSuccess", "You are successfully registered. You can now log-in");
                    }
                    else
                    {
                        await Clients.Caller.SendAsync("RegisterFailed",
                            "Hmmm... Something went wrong. Please try again");
                    }
                }
                else
                {
                    await Clients.Caller.SendAsync("RegisterFailed",
                        "Username already taken. Please consider another one");
                }
            }
            catch (Exception e)
            {
                await Clients.Caller.SendAsync("RegisterFailed",
                    "An Database Error occurred with the following message: " + e);
            }
        }

        #endregion
        #region Userprofile

        public async Task CheckPlayerExists(string id)
        {
            await Clients.Caller.SendAsync("ReceiveUserExists", PersistenceStorage.CheckPlayerIdIsUnique(id));
        }
        public async Task ChangeIngameName(string id, string name)
        {
            PersistenceStorage.UpdateInGameName(id, name);
            await Clients.Caller.SendAsync("NameIsChanged", name);
        }
        public async Task ChangePassword(string id, byte[]newSalt, byte[]newHash)
        {
            PersistenceStorage.UpdateSaltHash(id, newSalt, newHash);
            await Clients.Caller.SendAsync("PasswordIsChanged");
        }
        public async Task ChangeSymbolColor(string id, string color)
        {
            Color c = ColorTranslator.FromHtml(color);
            PersistenceStorage.UpdateColor(id, c);
            await Clients.Caller.SendAsync("ColorIsChanged", color);
        }
        public async Task ChangePlayerSymbol(string id, byte symbol)
        {
            PersistenceStorage.UpdatePlayerSymbol(id, symbol);
            await Clients.Caller.SendAsync("SymbolIsChanged", symbol);
        }
        public async Task GetPlayer(string id)
        {
            HumanPlayer player = PersistenceStorage.LoadPlayer(id);
            string jsonPlayer = JsonConvert.SerializeObject(player);
            await Clients.Caller.SendAsync("ReceivePlayer", jsonPlayer);
        }
        public async Task GetStats(string id)
        {
            PlayerStats stats = PersistenceStorage.GetPlayerStat(id);
            string jsonStats = JsonConvert.SerializeObject(stats);
            await Clients.Caller.SendAsync("ReceiveStats", jsonStats);
        }
        #endregion
        #region Lobbies

        public async Task GetCurrentLobbies()
        {
            var list = new LobbyList().Lobbies;
            
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var jsonList = JsonConvert.SerializeObject(list, Formatting.Indented, settings);
            await Clients.Caller.SendAsync("ReceiveCurrentLobbies", jsonList);
        }

        #endregion
    }
}