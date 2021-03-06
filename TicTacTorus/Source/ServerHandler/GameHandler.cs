﻿using System;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.Ingame.Move;
using TicTacTorus.Source.PlayerSpecificContent;

namespace TicTacTorus.Source.ServerHandler
{
    public static class GameHandler
    {
        #region Ingame

        public static Tuple<bool, string, byte> PlaceMove(string gameId, IMove move)
        {
            var game = Server.Instance.GetClientGameById(gameId);
            return game.SendMoveToGame(move);
        }

        #endregion
        #region FromGame

        

        #endregion
        #region Init

        public static ClientGame CreateGame(string lobbyId, IHubCallerClients hubClients)
        {
            return Server.Instance.CreateGameFromLobby(lobbyId, hubClients);
        }


        public static Tuple<ClientGame, bool, int> AddPlayerToGame(string gameId, IPlayer player)
        {
            var game = Server.Instance.GetClientGameById(gameId);
            return Tuple.Create(game, game.AddPlayer(player), game.players.IndexOf(player));
        }

        #endregion
    }
}