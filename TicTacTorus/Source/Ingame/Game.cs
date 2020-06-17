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

        public GameSettings Settings { get; }

        private List<IPlayer> _players = new List<IPlayer>();
        private bool _hasStarted = false;
        public Permutation PlayerOrder { get; }
        private byte _activePlayerIndex;

        private IGrid _grid;
        private IReferee _referee;

        public IList<IMove> MoveHistory { private set; get; }

        public ClientGame Parent;

        #endregion
        #region Constructors

        public Game(ILobby lobby)
        {
            StartTime = DateTime.Now;
            ID = lobby.Id;
            _players = lobby.GetAllPlayers();

            PlayerOrder = Permutation.Random(_players.Count);
            
            Settings = lobby.Settings;
            _grid = new Grid(Settings.GridSize, Settings.GridSize);
            _referee = new LineReferee(Settings.WinChainLength);
        }
        
        #endregion
        #region Game Loop

        /*
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
            *
            
            var lastChange = new GlobalPos(0, 0);
            IPlayer lastPlayer = null;
            do
            {
                lastPlayer = ChoosePlayer();
                IMove move = null;

                var connection = _clients.Group(UniquePlayerGroup(ID, _activePlayerIndex));
                do
                {
                    //todo: discuss exactly how to do IMove exchange. ReceivePlayerMove might be an idea, but that means we have to restructure our main loop.
                    
                    move = lastPlayer.ChooseMove(connection, _grid, Settings.TimeLimitSec);
                } while (move != null && !move.CanDo(_grid, PlayerOrder));
                move.Do(_grid, PlayerOrder);
                MoveHistory.Add(move);

                NextPlayer();
            } while (!_referee.HasWon(_grid, lastChange));

            await _clients.Group(ID.ToString()).SendAsync("AnnounceWinner");
        }
        */
        
        #endregion
        #region Communication

        public void ReceivePlayerMove(IPlayer plr, IMove move)
        {
            try
            {
                var index = _players.IndexOf(plr);
                ReceivePlayerMove(index, move);
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        public void ReceivePlayerMove(int plrIndex, IMove move)
        {
            if (plrIndex != _activePlayerIndex)
            {
                return;
            }
            if (!move.CanDo(_grid, PlayerOrder))
            {
                //send error message to player
                Parent.DenyMove(plrIndex);
                return;
            }

            //send move to all users
            Parent.DistributeMove(plrIndex, move);

            //check the changed area for any winner(s).
            var winners = new Dictionary<byte, GlobalPos>();
            var width = move.GetAreaWidth();
            var height = move.GetAreaHeight();
            if (width > 0 && height > 0)
            {
                var start = move.GetAreaCorner();
                var pos = new GlobalPos(start.X, start.Y);
                for (var y = 0; y < height; ++y)
                {
                    pos.X = start.X;
                    for (var x = 0; x < width; ++x)
                    {
                        if (_referee.HasWon(_grid, pos))
                        {
                            var owner = _grid.GetSymbol(pos);
                            _grid.SetSymbol(pos, owner);
                            if (!winners.ContainsKey(owner))
                            {
                                winners[owner] = new GlobalPos();
                            }
                        }
                        ++pos.X;
                    }
                    ++pos.Y;
                }
            }
/*
            if (winners.Count > 0)
            {
                Parent.AnnounceWinners(winners);
            }*/
            NextPlayer();
        }

        #endregion
        #region Helper Methods
        
        private IPlayer ChooseNextPlayer()
        {
            NextPlayer();
            return ChoosePlayer();
        }

        private IPlayer ChoosePlayer()
        {
            return _players[PlayerOrder[_activePlayerIndex]];
        }

        private void NextPlayer()
        {
            //this loop terminates once iterated through the whole list (and returns null in this case)
            for (var i = 0; i < _players.Count && _players[PlayerOrder[_activePlayerIndex]] == null; ++i)
            {
                ++_activePlayerIndex;
                _activePlayerIndex %= (byte)_players.Count;
            }
        }
        
        public static string UniquePlayerGroup(Base64 gameID, int playerIndex)
        {
            return $"{gameID}::{playerIndex}";
        }
        public static string UniquePlayerGroup(string gameID, int playerIndex)
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