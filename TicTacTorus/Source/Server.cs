using System.Buffers.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.LobbySpecificContent;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.ServerHandler;
using Base64 = TicTacTorus.Source.Utility.Base64;

namespace TicTacTorus.Source
{
    public sealed class Server
    {
        private readonly IDictionary<string, ILobby> _lobbies;
        private readonly IDictionary<string, Game> _games;

        private readonly IDictionary<string, LobbyGame> _lobbygames;    /// <summary>
                                                                        /// If it works, then games in this server-list
                                                                        /// are unneccesary, as games are contained in LobbyGame
                                                                        /// </summary>
        
        //private readonly IDictionary<string, string> _sessionIDs;

        public ServerSettings Settings { get; }

        #region Instance

        // For being Thread-safe ("full lazy instantiation" - https://csharpindepth.com/articles/singleton)
        public static Server Instance => Nested.Instance;

        private Server()
        {
            _lobbies = new ConcurrentDictionary<string, ILobby>();
            _games = new ConcurrentDictionary<string, Game>();
            _lobbygames = new ConcurrentDictionary<string, LobbyGame>();
            //_sessionIDs = new ConcurrentDictionary<string, string>();
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

        #region LobbyGame

        public LobbyGame CreateLobbyGameFromLobby(string lobbyId)
        {
            var lobby = GetLobbyById(lobbyId);
            if (_games.ContainsKey(lobbyId) || !_lobbies.Remove(lobbyId)) return null;
            // Delete Player list first
            lobby.Players = new List<IPlayer>();
                
            var game = new Game(lobby);
            _games.Add(game.ID.ToString(), game);
            
            var lgame = new LobbyGame(game);
            _lobbygames.Add(lgame.ID, lgame);

            return lgame;
        }

        public LobbyGame GetLobbyGameById(string id)
        {
            return _lobbygames[id];
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
            if (!_games.ContainsKey(lobby.Id.ToString()) && _lobbies.Remove(lobby.Id.ToString()))
            {
                game = new Game(lobby);
                _games.Add(game.ID.ToString(), game);
            }

            return game;
        }
        
        public Game CreateGameFromLobby(string lobbyId)
        {
            var lobby = GetLobbyById(lobbyId);
            if (_games.ContainsKey(lobbyId) || !_lobbies.Remove(lobbyId)) return null;
            // Delete Player list first
            lobby.Players = new List<IPlayer>();
                
            var game = new Game(lobby);
            _games.Add(game.ID.ToString(), game);

            return game;
        }

        #endregion
/*
        #region SessionID

        //Creates new Session ID and returns it only if ID is not already taken
        public string GetSessionId(string userId)
        {
            string sId;
            if (_sessionIDs.ContainsKey(userId))
            {
                return _sessionIDs[userId];
            }

            while (!_sessionIDs.Values.Contains(sId = Base64.Random(16).ToString()))
            {
                _sessionIDs.Add(userId, sId);
                break;
            }
            return _sessionIDs[userId];
        }

        public void RemoveSessionId(string userId)
        {
            _sessionIDs.Remove(userId);
        }

        #endregion
        */
    }
}