using System;
using Microsoft.AspNetCore.SignalR;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.PlayerSpecificContent;

namespace TicTacTorus.Source.ServerHandler
{
    public static class GameHandler
    {



        #region Init

        public static LobbyGame CreateGame(string lobbyId, IHubCallerClients clients)
        {
            return Server.Instance.CreateLobbyGameFromLobby(lobbyId, clients);
        }


        public static Tuple<LobbyGame, bool> AddPlayerToGame(string gameId, IPlayer player)
        {
            var game = Server.Instance.GetLobbyGameById(gameId);
            return Tuple.Create(game, game.AddPlayer(player));
        }

        #endregion
    }
}