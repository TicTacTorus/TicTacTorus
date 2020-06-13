using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TicTacTorus.Source.Hubs;
using TicTacTorus.Source.Ingame.GridSpecificContent.Chunk;
using TicTacTorus.Source.Ingame.GridSpecificContent.Grid;
using TicTacTorus.Source.Ingame.GridSpecificContent.Position;
using TicTacTorus.Source.Ingame.Move;
using TicTacTorus.Source.Ingame.Referee;
using TicTacTorus.Source.LobbySpecificContent;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.ServerHandler;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Ingame
{
    public class Game
    {
        #region Fields
        
        public Base64 ID { get; }
        public DateTime StartTime { get; }
        public int MoveSeconds { get; }

        private List<IPlayer> _players = new List<IPlayer>();
        private bool _hasStarted = false;
        public Permutation PlayerOrder { get; }
        private byte _activePlayerIndex;

        private IGrid _grid;
        private IReferee _referee;

        public IList<IMove> MoveHistory { private set; get; }

        private IHubCallerClients _clients;
        
        #endregion
        #region Constructors

        public Game(ILobby lobby, IHubCallerClients clients)
        {
            StartTime = DateTime.Now;
            ID = lobby.Id;
            _players = lobby.GetAllPlayers();
            _clients = clients;
            
            //todo give us sensible config values from the lobby
            _grid = new Grid(50, 50);
            _referee = new LineReferee(5);
        }
        
        #endregion
        #region Game Loop

        public async Task Run()
        {
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
                var playerIndex = _activePlayerIndex;
                lastPlayer = ChooseNextPlayer();
                IMove move = null;

                var connection = _clients.Group(UniquePlayerGroup(ID, playerIndex));
                do
                {
                    move = lastPlayer.ChooseMove(connection, _grid, MoveSeconds);
                } while (move != null && !move.CanDo(_grid, PlayerOrder));
                move.Do(_grid, PlayerOrder);
                MoveHistory.Add(move);
            } while (!_referee.HasWon(_grid, lastChange));

            await _clients.Group(ID.ToString()).SendAsync("AnnounceWinner");
            
        }
        
        #endregion
        #region Helper Methods

        private IPlayer ChooseNextPlayer()
        {
            IPlayer result = null;
            //this loop terminates once iterated through the whole list (and returns null in this case)
            for (var i = 0; i < _players.Count && result == null; ++i)
            {
                result = _players[PlayerOrder[_activePlayerIndex]];
                ++_activePlayerIndex;
                _activePlayerIndex %= (byte)_players.Count;
            }
            return result;
        }

        public static string UniquePlayerGroup(Base64 gameID, int playerIndex)
        {
            return $"{gameID}::{playerIndex}";
        }
        
        #endregion
        #region Field Changing Methods

        public bool AddPlayer(IPlayer player)
        {
            if (_hasStarted) return false;
            _players.Add(player);
            return true;

        }

        public List<IPlayer> GetPlayerList()
        {
            return _players;
        }

        public void StartGame()
        {
            _hasStarted = true;
            // Do some more init things probably
        }
        
        #endregion
    }
}