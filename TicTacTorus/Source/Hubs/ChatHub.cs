using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TicTacTorus.Source.Hubs
{
    public class ChatHub : Hub
    {
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