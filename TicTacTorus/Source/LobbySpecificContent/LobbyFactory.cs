﻿namespace TicTacTorus.Source.LobbySpecificContent
{
    public class LobbyFactory
    {
        public static ILobby CreateLobby(IPlayer owner)
        {
            return new Lobby(CreateLobbyName(owner), owner, CreateLobbyStatus(),
                CreateLobbyDescription(owner), 5, true);
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