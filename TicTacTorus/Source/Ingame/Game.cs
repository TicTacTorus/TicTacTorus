using System;
using System.Collections.Generic;
using TicTacTorus.Source.LobbySpecificContent;

namespace TicTacTorus.Source.Ingame
{
    public class Game
    {
        public DateTime StartTime { get; private set; }
        public int MoveSeconds { get; set; }
        
        private IList<IPlayer> _players;

        public Game(ILobby Lobby)
        {
            StartTime = DateTime.Now;
            _players = Lobby.GetAllPlayers();
        }
    }
}