using System;
using System.Collections.Generic;
using System.Linq;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.LobbySpecificContent
{
    public class LobbyList
    {
        public IDictionary<string, ILobby> Lobbies { get; protected set; }

        public LobbyList()
        {
            UpdateLobbies();
        }

        private IDictionary<string, ILobby> FetchAllActiveLobbies()
        {
            var lobbies = Server.Instance.GetPublicLobbies();

            return lobbies;
        }

        public void UpdateLobbies()
        {
            Lobbies = FetchAllActiveLobbies();
        }
        
    }
}