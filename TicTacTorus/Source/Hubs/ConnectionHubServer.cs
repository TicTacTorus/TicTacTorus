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

namespace TicTacTorus.Source.Hubs
{
    public class ConnectionHubServer : Hub
    {
        #region Lobby
        public async Task CreateLobby()
        {
            var server = Server.Instance;
            var hp = PlayerFactory.CreateHumanPlayer();
            var lobby = LobbyFactory.CreateLobbyWithId(hp);
            server.AddLobby(lobby);
            await Clients.Caller.SendAsync("ReceiveLobbyId", lobby.Id.ToString());
        }
        /* later
        public async Task RemovePlayerFromLobby(string lobbyId, IPlayer player)
        {
            Server.Instance.GetLobbyById(lobbyId).RemovePlayer(player);
            // signal everyone, that player is removed (=false)
            await Clients.Group(lobbyId.ToString()).SendAsync("PlayerListChanged", player, false);    
        }
        public async Task AddPlayerToLobby(string lobbyId, IPlayer player)
        {
            Server.Instance.GetLobbyById(lobbyId).AddPlayer(player);
            // signal everyone, that player is added (=true)
            await Clients.Group(lobbyId.ToString()).SendAsync("PlayerListChanged", player, true);    
        }*/
        public async Task JoinLobby(string lobbyId)
        {
            var lobby = Server.Instance.GetLobbyById(lobbyId);
            var jsLobby = JsonConvert.SerializeObject(lobby);
            await Clients.Caller.SendAsync("GetLobby", jsLobby);
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
        }

        public Task LeaveLobby(string lobbyId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);
        }
        #endregion
        #region Lobbies

        public async Task GetCurrentLobbies()
        {
            var list = new LobbyList().Lobbies;
            var lobbiesJson = JsonConvert.SerializeObject(list);
            await Clients.Caller.SendAsync("ReceiveCurrentLobbies", lobbiesJson);
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

        public async Task ConfirmRegister(HumanPlayer user)
        {
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
    }
}