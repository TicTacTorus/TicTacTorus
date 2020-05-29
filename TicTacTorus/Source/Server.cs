using System.Buffers.Text;
using System.Collections.Generic;
using System.Net.Sockets;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.LobbySpecificContent;
using Base64 = TicTacTorus.Source.Utility.Base64;

namespace TicTacTorus.Source
{
    public sealed class Server
    {
        private Dictionary<string,ILobby> _lobbies = new Dictionary<string, ILobby>();
        private Dictionary<string,Game> _games = new Dictionary<string, Game>();
        // For being Thread-safe ("full lazy instantiation" - https://csharpindepth.com/articles/singleton)
        public static Server Instance { get { return Nested.instance; } }
        private Server()
        {
            _lobbies = new Dictionary<string, ILobby>();
            _games = new Dictionary<string, Game>();
        }
        // Makes Singleton Thread-safe
        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly Server instance = new Server();
        }
        
    

        #region Lobby

        public bool AddLobby(ILobby lobby)
        {
            if (LobbyIdIsUnique(lobby.Id.ToString()))
            {
                _lobbies.Add(lobby.Id.ToString() ,lobby);
                return true;
            }

            return false;
        }

        public ILobby GetLobbyById(string id)
        {
            return _lobbies[id];
        }

        public bool LobbyIdIsUnique(string id)
        {
            return !_lobbies.ContainsKey(id);
        }

        #endregion
        #region Game

        public bool AddGame(Game game)
        {
            if (GameIdIsUnique(game.ID.ToString()))
            {
                _games.Add(game.ID.ToString(), game);
                return true;
            }

            return false;
        }

        public Game GetGameById(string id)
        {
            return _games[id];
        }

        public bool GameIdIsUnique(string id)
        {
            return _games.ContainsKey(id);
        }

        #endregion
        
        /// <summary>
        /// Create and save a new Instance of Game based on ILobby.
        /// </summary>
        /// <param name="lobby">An ILobby instance</param>
        /// <returns>
        /// new Game(lobby), null if ID of lobby was not unique or lobby could not be removed from list
        /// </returns>
        public Game CreateGameFromLobby(ILobby lobby)
        {
            Game game = null;
            if (_games.ContainsKey(lobby.Id.ToString()) && _lobbies.Remove(lobby.Id.ToString()))
            {
                game = new Game(lobby);
                _games.Add(game.ID.ToString(), game);
            }

            return game;
        }
    }
}