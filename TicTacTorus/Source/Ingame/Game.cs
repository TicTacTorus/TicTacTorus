using System;
using System.Collections.Generic;
using TicTacTorus.Source.Ingame.GridSpecificContent.Chunk;
using TicTacTorus.Source.Ingame.GridSpecificContent.Grid;
using TicTacTorus.Source.Ingame.GridSpecificContent.Position;
using TicTacTorus.Source.Ingame.Move;
using TicTacTorus.Source.Ingame.Referee;
using TicTacTorus.Source.LobbySpecificContent;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Ingame
{
    public class Game
    {
        #region Fields
        
        public Base64 ID { get; }
        public DateTime StartTime { get; }
        public int MoveSeconds { get; }

        private IList<IPlayer> _players = new List<IPlayer>();
        public Permutation PlayerOrder { get; }
        private byte _activePlayerIndex;

        private IGrid _grid;
        private IReferee _referee;

        public IList<IMove> MoveHistory { private set; get; }

        #endregion
        #region Constructors

        public Game(ILobby lobby)
        {
            StartTime = DateTime.Now;
            _players = lobby.GetAllPlayers();
            
            //todo give us sensible config values from the lobby
            _grid = new Grid(50, 50);
            _referee = new LineReferee(5);
        }
        
        #endregion
        #region Game Loop

        public void Run()
        {
            //todo something something starting thread? or is it just using a thread that lobby started?

            /*
                --- first rough concept ---
                start move timer (if finite)
                choose player
                    ask player for move
                    validate move
                    repeat until move valid
                apply move
                add move to history
                repeat until won
                at any time, somehow reset the move cycle if the timer stops
            */
            
            var lastChange = new GlobalPos(0, 0);
            IPlayer lastPlayer = null;
            do
            {
                lastPlayer = ChooseNextPlayer();
                IMove move = null;
                do
                {
                    //todo debate: if we save the connection inside the player we could create a ChooseMove() method inside the player.
                    //move = lastPlayer.ChooseMove(_grid, MoveSeconds);
                } while (move != null && move.CanDo(_grid, PlayerOrder));
                move.Do(_grid, PlayerOrder);
                MoveHistory.Add(move);
            } while (!_referee.HasWon(_grid, lastChange));

            //todo we have a winner!
            
        }
        
        #endregion
        #region Helper Methods

        private IPlayer ChooseNextPlayer()
        {
            IPlayer result = _players[PlayerOrder[_activePlayerIndex]];
            ++_activePlayerIndex;
            if (_activePlayerIndex >= _players.Count)
            {
                _activePlayerIndex = 0;
            }
            return result;
        }
        
        #endregion
    }
}