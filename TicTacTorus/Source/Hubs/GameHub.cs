using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TicTacTorus.Source.Generator;
using TicTacTorus.Source.LobbySpecificContent;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Hubs
{
    public class GameHub : Hub
    {
        public async Task CreateLobby()
        {
            Server server = Server.Instance;
            HumanPlayer hp = PlayerFactory.CreateHumanPlayer();
            ILobby lobby = LobbyFactory.CreateLobbyWithId(hp);
            server.AddLobby(lobby);
            await Clients.Caller.SendAsync("ReceiveLobbyId", lobby.Id.ToString());
        }
        public async Task SendMessage(string lobbyId, string user, string message)
        {
            await Clients.Group(lobbyId).SendAsync("ReceiveMessage", user, message);
        }

        public async Task RemovePlayerFromLobby(Base64 lobbyId, IPlayer player)
        {
            Server.Instance.GetLobbyById(lobbyId).RemovePlayer(player);
            // signal everyone, that player is removed (=false)
            await Clients.Group(lobbyId.ToString()).SendAsync("PlayerListChanged", player, false);    
        }
        public async Task AddPlayerToLobby(Base64 lobbyId, IPlayer player)
        {
            Server.Instance.GetLobbyById(lobbyId).AddPlayer(player);
            // signal everyone, that player is added (=true)
            await Clients.Group(lobbyId.ToString()).SendAsync("PlayerListChanged", player, true);    
        }

        public async Task JoinLobby(string lobbyId)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
            ILobby lobby = Server.Instance.GetLobbyById(new Base64(lobbyId));
            await Clients.Caller.SendAsync("GetLobby", lobby);
            //return Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
        }

        public Task LeaveLobby(string lobbyId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);
        }
    }
}