using System;
using System.Collections.Generic;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.LobbySpecificContent
{
    public class LobbyList
    {
        public Dictionary<string, ILobby> Lobbies { get; }

        public LobbyList()
        {
            Lobbies = FetchAllActiveLobbies();
        }
        
        private Dictionary<string, ILobby> FetchAllActiveLobbies()
        {
            //Need to fetch all currently open Lobby Hubs that is
            //stored in the external storage?
            return MockLobbyList.GetAllLobbies();
        }
    }
}