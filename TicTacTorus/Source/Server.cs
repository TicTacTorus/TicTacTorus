using System.Buffers.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.LobbySpecificContent;
using Base64 = TicTacTorus.Source.Utility.Base64;

namespace TicTacTorus.Source
{
    public sealed class Server
    {
        private readonly IDictionary<string, ILobby> _lobbies;
        private readonly IDictionary<string, Game> _games;

        public ServerSettings Settings { get; }

        #region Instance

        // For being Thread-safe ("full lazy instantiation" - https://csharpindepth.com/articles/singleton)
        public static Server Instance => Nested.Instance;

        private Server()
        {
            _lobbies = new ConcurrentDictionary<string, ILobby>();
            _games = new ConcurrentDictionary<string, Game>();
        }
        // Makes Singleton Thread-safe
        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            // ReSharper disable once MemberHidesStaticFromOuterClass
            internal static readonly Server Instance = new Server();
        }
        
        public IDictionary<string,ILobby> Lobbies
        {
            get { return _lobbies; }
        }
        public IDictionary<string,Game> Games
        {
            get { return _games; }
        }

        #endregion
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

        public IDictionary<string, ILobby> GetPublicLobbies()
        {
            return _lobbies.Where(kvp => kvp.Value.IsPrivate)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
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
        #region ConvertLobbyToGame

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

        #endregion
        
    }
}