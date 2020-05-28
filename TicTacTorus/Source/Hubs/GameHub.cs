using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TicTacTorus.Source.Generator;
using TicTacTorus.Source.LobbySpecificContent;

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
            await Clients.Caller.SendAsync("ReceiveLobbyId", lobby.Id);
        }
        public async Task SendMessage(string lobbyId, string user, string message)
        {
            await Clients.Group(lobbyId).SendAsync("ReceiveMessage", user, message);
        }

        public Task JoinLobby(string lobbyId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
        }
        
        public Task LeaveLobby(string lobbyId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);
        }
    }
}