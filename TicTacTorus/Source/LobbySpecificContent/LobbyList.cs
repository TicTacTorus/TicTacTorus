using System.Collections.Generic;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.LobbySpecificContent
{
    public class LobbyList
    {
        public Dictionary<Base64, ILobby> Lobbies { get; private set; }

        public LobbyList()
        {
            Lobbies = FetchAllActiveLobbies();
        }
        
        private Dictionary<Base64, ILobby> FetchAllActiveLobbies()
        {
            //Need to fetch all currently open Lobby Hubs that is
            //stored in the external storage?
            return MockLobbyList.GetAllLobbies();
        }
    }
}