using System;
using System.Data.SQLite;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TicTacTorus.Source.Ingame.Move;
using TicTacTorus.Source.LobbySpecificContent;
using TicTacTorus.Source.Persistence;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.ServerHandler;

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
        
        public async Task JoinLobby(string lobbyId, string player)
        {
            var hPlayer = JsonConvert.DeserializeObject<HumanPlayer>(player);
            hPlayer.SessionID = Context.ConnectionId;
            var lobby = LobbyHandler.AddPlayerToLobby(lobbyId, hPlayer);

            var indented = Formatting.Indented;
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var jsLobby = JsonConvert.SerializeObject(lobby, indented, settings);
            await Clients.Caller.SendAsync("GetLobby", jsLobby, Context.ConnectionId);
            await Clients.Group(lobbyId).SendAsync("LobbyChanged", jsLobby);
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
        }
        
        public async Task RemovePlayerFromLobby(string lobbyId, string player)
        {
            var p = JsonConvert.DeserializeObject<IPlayer>(player);
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

        public async Task CheckLobbyExisting(string id)
        {
            var exists = !Server.Instance.LobbyIdIsUnique(id);
            await Clients.Caller.SendAsync("ReceiveLobbyExisting", exists);
        }

        public Task LeaveLobby(string lobbyId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);
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
                //await Clients.Group(gameId).SendAsync("ReceiveGameInformation", jsGame);
                await Clients.Caller.SendAsync("ReceiveGameInformation", jsGame);
            }
            else
            {
                await Clients.Caller.SendAsync("ConnectToGameFailed", "game_has_started");
            }
        }
        /*
        public async Task JoinGame(string lobbyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
        }*/
        
        #endregion
        #region Game

        public void ReceivePlayerMove(IMove move)
        {
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

        /*public async Task GetPlayerStats(string userId, string playerId)
        {
            try
            {
                var player = PersistenceStorage.LoadPlayer(playerId);
                var playerStats = PersistenceStorage.GetPlayerStat(player);
               
                // filter some information
                player.Hash = null;
                player.Salt = null;
                player.playerStats = null;
            
                var jsonPlayer = JsonConvert.SerializeObject(player);
                var jsonStats = JsonConvert.SerializeObject(playerStats);

                await Clients.Caller.SendAsync("ReceiveStats", jsonPlayer,jsonStats);
            }
            catch
            {
                await Clients.Caller.SendAsync("Error", "An Error occurred. Please try again later.");
                return;
            }
            
        }*/

        /*
        //Gets called everytime a user connects to a hub (I know performance is not good >.<)
        public async Task GetSessionID(string userId)
        {
            await Clients.Caller.SendAsync("ReceiveSessionID", Server.Instance.GetSessionId(userId));
        }

        //Gets called everytime a user lefts a hub (I know is also bad :| )
        public void RemoveSessionID(string userId)
        {
            Server.Instance.RemoveSessionId(userId);
        }*/

        #endregion
        #region Userprofile

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
            await Clients.Caller.SendAsync("ReceiveCurrentLobbies", list);
        }

        #endregion
    }
}