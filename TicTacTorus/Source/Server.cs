using System.Buffers.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Microsoft.AspNetCore.SignalR;
using TicTacTorus.Source.Hubs;
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
        private readonly IDictionary<string, ClientGame> _lobbygames;   
        
        //private readonly IDictionary<string, string> _sessionIDs;

        public ServerSettings Settings { get; }
        
        #region Instance

        // For being Thread-safe ("full lazy instantiation" - https://csharpindepth.com/articles/singleton)
        public static Server Instance => Nested.Instance;

        private Server()
        {
            _lobbies = new ConcurrentDictionary<string, ILobby>();
            _lobbygames = new ConcurrentDictionary<string, ClientGame>();
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
        
        public IDictionary<string,ILobby> Lobbies => _lobbies;
        public IDictionary<string, ClientGame> LobbyGames => _lobbygames;

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
        #region LobbyGame

        public ClientGame GetLobbyGameById(string id)
        {
            return _lobbygames[id];
        }

        public bool GameIdIsUnique(string id)
        {
            return _lobbygames.ContainsKey(id);
        }
        
        public Game GetGameById(string id)
        {
            return _lobbygames[id].Game;
        }
        
        public ClientGame AddGame(Game game)
        {
            if (GameIdIsUnique(game.ID.ToString()))
            {
                var lg = new ClientGame(game);
                _lobbygames.Add(game.ID.ToString(), lg);
                return lg;
            }

            return null;
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
        public ClientGame CreateGameFromLobby(string lobbyId, IHubCallerClients clients)
        {
            var lobby = GetLobbyById(lobbyId);
            if (_lobbygames.ContainsKey(lobbyId) || !_lobbies.Remove(lobbyId)) return null;
            // Delete Player list first
            lobby.Players = new List<IPlayer>();
                
            var game = new Game(lobby, clients);
            var lgame = new ClientGame(game);
            
            _lobbygames.Add(lgame.ID, lgame);

            return lgame;
        }

        #endregion
    }
}