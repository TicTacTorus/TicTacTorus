using System.Collections.Generic;

namespace TicTacTorus.Source.LobbySpecificContent
{
    public class LobbyList
    {
        public Dictionary<string, ILobby> Lobbies { get; private set; }

        public LobbyList()
        {
            Lobbies = FetchAllActiveLobbies();
        }
        
        public Dictionary<string, ILobby> FetchAllActiveLobbies()
        {
            //Need to fetch all currently open Lobby Hubs that is
            //stored in the external storage?
            return null;
        }
    }
}