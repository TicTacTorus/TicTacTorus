using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TicTacTorus.Source.Generator;
using TicTacTorus.Source.LobbySpecificContent;
using TicTacTorus.Source.LoginContent.Security;
using TicTacTorus.Source.Persistence;
using TicTacTorus.Source.PlayerSpecificContent;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TicTacTorus.Source.Hubs
{
    public class ConnectionHubServer : Hub
    {
        #region Lobby
        public async Task CreateLobby(HumanPlayer hp)
        {
            var lobby = LobbyHandler.CreateLobby(hp);
            await Clients.Caller.SendAsync("ReceiveLobbyId", lobby.Id.ToString());
        }
        
        public async Task RemovePlayerFromLobby(string lobbyId, string player)
        {
            IPlayer p = JsonConvert.DeserializeObject<IPlayer>(player);
            var (isRemoved, lobby) = LobbyHandler.RemovePlayerFromLobby(lobbyId, p);

            if (isRemoved)
            {
                await Clients.Group(lobbyId).SendAsync("ReceiveMessage", p.InGameName, "I left the Lobby");
                
                await Clients.Group(lobbyId).SendAsync("LobbyChanged", JsonConvert.SerializeObject(lobby));
            }
            else
            {
                await Clients.Group(lobbyId).SendAsync("ReceiveMessage", "Game", "There was something wrong. "+p.InGameName+" couldn't be removed");
            }
        }
        
        public async Task JoinLobby(string lobbyId, string player)
        {
            var hPlayer = JsonConvert.DeserializeObject<HumanPlayer>(player);
            var lobby = LobbyHandler.AddPlayerToLobby(lobbyId, hPlayer);

            var indented = Formatting.Indented;
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var jsLobby = JsonConvert.SerializeObject(lobby, indented, settings);
            await Clients.Caller.SendAsync("GetLobby", jsLobby);
            await Clients.Group(lobbyId).SendAsync("LobbyChanged", jsLobby);
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
        }

        public Task LeaveLobby(string lobbyId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);
        }

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
        
        #endregion
        #region Lobbies

        public async Task GetCurrentLobbies()
        {
            var list = new LobbyList().Lobbies;
            await Clients.Caller.SendAsync("ReceiveCurrentLobbies", list);
        }

        #endregion
        #region Chat
        public async Task SendMessage(string lobbyId, string user, string message)
        {
            await Clients.Group(lobbyId).SendAsync("ReceiveMessage", user, message);
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

        public async Task GetPlayerStats(string userId, string playerId)
        {
            try
            {
                var player = PersistenceStorage.LoadPlayer(playerId);
                var playerStats = PersistenceStorage.GetPlayerStat(player);
                if (userId == null || userId != playerId)
                {
                    // filter some information
                    player.Hash = null;
                    player.Salt = null;
                    player.playerStats = null;
                
                    var jsonPlayer = JsonConvert.SerializeObject(player);
                    var jsonStats = JsonConvert.SerializeObject(playerStats);

                    await Clients.Caller.SendAsync("ReceiveStatsNoAuthorisation", jsonPlayer,playerStats);
                }
                else
                {
                    var jsonPlayer = JsonConvert.SerializeObject(player);
                    var jsonStats = JsonConvert.SerializeObject(playerStats);

                    await Clients.Caller.SendAsync("ReceiveStatsAsOwner", jsonPlayer, jsonStats);
                }
            }
            catch
            {
                await Clients.Caller.SendAsync("Error", "An Error occurred. Please try again later.");
                return;
            }
            
        }

        //Gets called everytime a user connects to a hub (I know performance is not good >.<)
        public async Task GetSessionID(string userId)
        {
            await Clients.Caller.SendAsync("ReceiveSessionID", Server.Instance.GetSessionId(userId));
        }

        //Gets called everytime a user lefts a hub (I know is also bad :| )
        public void RemoveSessionID(string userId)
        {
            Server.Instance.RemoveSessionId(userId);
        }

        #endregion
        #region User

        public void ChangeIngameName(string id, string name)
        {
            PersistenceStorage.UpdateInGameName(id, name);
        }

        #endregion
    }
}