﻿﻿using Microsoft.VisualBasic.CompilerServices;
 using TicTacTorus.Source.PlayerSpecificContent;
 using TicTacTorus.Source.Utility;

 namespace TicTacTorus.Source.LobbySpecificContent
{
    public class LobbyFactory
    {
        public static ILobby CreateLobbyNoId(IPlayer owner)
        {
            return new Lobby(CreateLobbyName(owner), owner, CreateLobbyStatus(),
                CreateLobbyDescription(owner), 5, true);
        }
        
        public static ILobby CreateLobbyWithId(IPlayer owner)
        {
            ILobby lobby = new Lobby(CreateLobbyName(owner), owner, CreateLobbyStatus(),
                CreateLobbyDescription(owner), 5, true);
            lobby = AddId(lobby);
            return lobby;
        }

        private static ILobby AddId(ILobby l)
        {
            Utility.Base64 id = new Base64();
            if (Server.Instance.LobbyIdIsUnique(id))
            {
                l.Id = id;
                return l;
            }
            return AddId(l);
        }

        private static string CreateLobbyName(IPlayer owner)
        {
            return owner.IngameName + "'s Game";
        }

        private static string CreateLobbyStatus()
        {
            return "just opened";
        }

        private static string CreateLobbyDescription(IPlayer owner)
        {
            return owner.IngameName + "'s Description";
        }
    }
}