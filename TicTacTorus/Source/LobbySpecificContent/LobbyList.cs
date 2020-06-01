using System;
using System.Collections.Generic;
using System.Linq;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.LobbySpecificContent
{
    public class LobbyList
    {
        public Dictionary<string, ILobby> Lobbies { get; protected set; }

        public LobbyList()
        {
            UpdateLobbies();
        }

        private Dictionary<string, ILobby> FetchAllActiveLobbies()
        {
            var lobbies = Server.Instance.Lobbies.Where(kvp => kvp.Value.IsPrivate)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return lobbies;
        }

        public void UpdateLobbies()
        {
            Lobbies = FetchAllActiveLobbies();
        }
        
    }
}