using System;
using System.Collections.Generic;
using System.Linq;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.LobbySpecificContent
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

        public void UpdateLobbies()
        {
            Lobbies = FetchAllActiveLobbies();
        }
        
    }
}