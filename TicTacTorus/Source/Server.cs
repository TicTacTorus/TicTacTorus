﻿using System.Buffers.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
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
        private readonly IDictionary<string, ClientGame> _games;
        private readonly IDictionary<string, HumanPlayer> _connectedUsers;
        //private readonly IDictionary<string, string> _sessionIDs;

        public ServerSettings Settings { get; }
        
        #region Instance

        // For being Thread-safe ("full lazy instantiation" - https://csharpindepth.com/articles/singleton)
        public static Server Instance => Nested.Instance;

        private Server()
        {
            _lobbies = new ConcurrentDictionary<string, ILobby>();
            _games = new ConcurrentDictionary<string, ClientGame>();
            _connectedUsers = new ConcurrentDictionary<string, HumanPlayer>();
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
        public IDictionary<string, ClientGame> ClientGames => _games;

        #endregion
        #region Lobby

        public bool AddLobby(ILobby lobby)
        {
            if (!LobbyIdIsUnique(lobby.Id.ToString())) return false;
            _lobbies.Add(lobby.Id.ToString() ,lobby);
            return true;

        }
        
        public void RemoveLobby(string lobbyId)
        {
            _lobbies.Remove(lobbyId);
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

        public ClientGame GetClientGameById(string id)
        {
            return _games[id];
        }

        private bool GameIdIsUnique(string id)
        {
            return _games.ContainsKey(id);
        }
        
        public Game GetGameById(string id)
        {
            return _games[id].Game;
        }
        
        public ClientGame AddGame(Game game)
        {
            if (!GameIdIsUnique(game.ID.ToString())) return null;
            var lg = new ClientGame(game);
            _games.Add(game.ID.ToString(), lg);
            return lg;

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
            if (_games.ContainsKey(lobbyId) || !_lobbies.Remove(lobbyId)) return null;
            // Delete Player list first
            lobby.Players = new List<IPlayer>();
                
            var game = new Game(lobby);
            var lgame = new ClientGame(game, clients);
            
            _games.Add(lgame.ID, lgame);

            return lgame;
        }

        #endregion

        public void AddConnectedUser(string connectionId, HumanPlayer user)
        {
            _connectedUsers.Add(connectionId, user);
        }

        public void RemoveConnectedUser(string connectionId)
        {
            _connectedUsers.Remove(connectionId);
        }

        public HumanPlayer GetConnectedUser(string connectionId)
        {
            return _connectedUsers[connectionId];
        }
    }
}