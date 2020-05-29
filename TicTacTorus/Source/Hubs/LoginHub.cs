using System;
using System.Data.SQLite;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TicTacTorus.Source.Hubs
{
    public class LoginHub : Hub
    {
        public async Task ConfirmLogin(string playerID, string playerPW)
        {
            // Check in Database
            HumanPlayer playerFromDatabase = null;
            try
            {
                // playerFromDatabase IPersistanceStorage.GetPlayer(playerID, playerPW);
                
                string playerJson = JsonConvert.SerializeObject(playerFromDatabase);
                await Clients.Client(Context.ConnectionId)
                    .SendAsync("ReceiveConfirmation", playerJson);
            }
            catch (SQLiteException e)
            {
                await Clients.Caller.SendAsync("LoginFailed", "Login failed. Wrong userID or Password.");
            }
            
        }
    }
}