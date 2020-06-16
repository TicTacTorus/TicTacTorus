using System.Collections.Generic;
using System.Linq;
using TicTacTorus.Source.LobbySpecificContent;

namespace TicTacTorus.Source.ServerHandler
{
    public class LobbyList
    {
        public List<ILobby> Lobbies { get; set; }

        public LobbyList()
        {
            UpdateLobbies();
        }

        private List<ILobby> FetchAllActiveLobbies()
        {
            var lobbies = Server.Instance.GetPublicLobbies().Values.ToList();
            
            return lobbies;
        }

        private void UpdateLobbies()
        {
            Lobbies = FetchAllActiveLobbies();
        }
        
    }
}