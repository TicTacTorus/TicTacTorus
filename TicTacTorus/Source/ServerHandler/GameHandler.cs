﻿using System;
using Microsoft.AspNetCore.SignalR;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.Ingame.Move;
using TicTacTorus.Source.PlayerSpecificContent;

namespace TicTacTorus.Source.ServerHandler
{
    public static class GameHandler
    {
        #region Ingame

        public static bool PlaceMove(string gameId, in byte playerIndex, IMove move)
        {
            var game = Server.Instance.GetClientGameById(gameId);
            game.Game.ReceivePlayerMove(playerIndex, move);

            return false;
        }

        #endregion
        #region FromGame

        

        #endregion
        #region Init

        public static ClientGame CreateGame(string lobbyId, IHubCallerClients clients)
        {
            return Server.Instance.CreateGameFromLobby(lobbyId, clients);
        }


        public static Tuple<ClientGame, bool> AddPlayerToGame(string gameId, IPlayer player)
        {
            var game = Server.Instance.GetClientGameById(gameId);
            return Tuple.Create(game, game.AddPlayer(player));
        }

        #endregion
    }
}