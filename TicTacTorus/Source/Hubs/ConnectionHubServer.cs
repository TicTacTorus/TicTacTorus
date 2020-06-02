using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TicTacTorus.Source.Generator;
using TicTacTorus.Source.LobbySpecificContent;
using TicTacTorus.Source.Persistence;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.Utility;

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

        #region Lobbies
        
        async Task GetCurrentLobbies()
        {
            var list = new LobbyList().Lobbies;
            var lobbies = new List<ILobby>(list.Values);
            var lobbiesJson = JsonConvert.SerializeObject(lobbies);
            await Clients.Caller.SendAsync("ReceiveCurrentLobbies", lobbiesJson);
        }
        
        #endregion

        #endregion
        #region Chat
        public async Task SendMessage(string lobbyId, string user, string message)
        {
            await Clients.Group(lobbyId).SendAsync("ReceiveMessage", user, message);
        }
        #endregion
        #region Login / Register

        public async Task ConfirmLogin(string playerId, string playerPw)
        {
            // Check in Database
            IPlayer playerFromDatabase = null;
            try
            {
                // playerFromDatabase IPersistanceStorage.GetPlayer(playerID, playerPW);
                playerFromDatabase = PersistenceStorage.LoadPlayer(playerId, playerPw);
                    
                string playerJson = JsonConvert.SerializeObject(playerFromDatabase);
                await Clients.Client(Context.ConnectionId)
                    .SendAsync("ReceiveConfirmation", playerJson);
            }
            catch (SQLiteException e)
            {
                await Clients.Caller.SendAsync("LoginFailed", "Login failed. Wrong userID or Password.");
            }

            await Clients.Client(Context.ConnectionId)
                .SendAsync("ReceiveConfirmation", playerFromDatabase);
        }

        #endregion
    }
}