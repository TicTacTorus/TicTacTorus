using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TicTacTorus.Source.Hubs
{
    public class LoginHub : Hub
    {
        public async Task ConfirmLogin(string playerID, string playerPW)
        {
            // Check in Database
            HumanPlayer playerFromDatabase = null;
            
            await Clients.Client(Context.ConnectionId)
                .SendAsync("ReceiveConfirmation", playerFromDatabase);
        }
    }
}