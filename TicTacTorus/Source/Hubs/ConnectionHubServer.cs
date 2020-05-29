using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TicTacTorus.Source.Generator;
using TicTacTorus.Source.LobbySpecificContent;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Hubs
{
    public class ConnectionHubServer : Hub
    {
        public async Task CreateLobby()
        {
            var server = Server.Instance;
            var hp = PlayerFactory.CreateHumanPlayer();
            var lobby = LobbyFactory.CreateLobbyWithId(hp);
            server.AddLobby(lobby);
            await Clients.Caller.SendAsync("ReceiveLobbyId", lobby.Id.ToString());
        }
        public async Task SendMessage(string lobbyId, string user, string message)
        {
            await Clients.Group(lobbyId).SendAsync("ReceiveMessage", user, message);
        }

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
        }

        public async Task JoinLobby(string lobbyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
            ILobby lobby = Server.Instance.GetLobbyById(lobbyId);
            string jsLobby = JsonConvert.SerializeObject(lobby);
            await Clients.Caller.SendAsync("GetLobby", jsLobby);
            //return Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
        }

        public Task LeaveLobby(string lobbyId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);
        }
    }
}