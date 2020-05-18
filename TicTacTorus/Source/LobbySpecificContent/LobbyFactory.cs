﻿namespace TicTacTorus.Source.LobbySpecificContent
{
    public class LobbyFactory
    {
        public LobbyFactory()
        {
            
        }

        public ILobby CreateLobby(IPlayer player)
        {
            return new Lobby(CreateLobbyName(player),CreateLobbyStatus(),
                CreateLobbyDescription(player), 5, true);
        }

        private string CreateLobbyName(IPlayer player)
        {
            return player.IngameName + "'s Game";
        }

        private string CreateLobbyStatus()
        {
            return "just opened";
        }

        private string CreateLobbyDescription(IPlayer player)
        {
            return player.IngameName + "'s Description";
        }
    }
}