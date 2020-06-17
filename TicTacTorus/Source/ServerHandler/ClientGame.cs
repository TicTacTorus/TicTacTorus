using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TicTacTorus.Source.Hubs;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.Ingame.GridSpecificContent.Position;
using TicTacTorus.Source.Ingame.Move;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.ServerHandler
{
    public class ClientGame
    {
        [JsonIgnore]
        public Game Game { get; }

        public string ID { get; set; }
        public List<IPlayer> players { get; set; }
        public int[] PlayerOrder { get; set; }
        public GameSettings Settings { get; set; }

        [JsonIgnore]
        private IHubCallerClients _hubClients;

        public ClientGame(Game game)
        {
            Game = game;
            Game.Parent = this;
            ID = game.ID.ToString();
            players = Game.GetPlayerList();
            PlayerOrder = game.PlayerOrder.ToArray();
            Settings = game.Settings;
        }
        
        public ClientGame(Game game, IHubCallerClients hub) : this(game)
        {
            _hubClients = hub;
        }

        public ClientGame()
        {
            players = new List<IPlayer>();
        }

        #region Accessors

        public bool AddPlayer(IPlayer p)
        {
            if (!Game.AddPlayer(p)) return false;
            players.Add(p);
            return true;
        }

        #endregion

        #region Moves

        public async void DenyMove(int plrIndex)
        {
            var group = Game.UniquePlayerGroup(Game.ID, plrIndex);
            await _hubClients.Group(group).SendAsync("ReceiveAlert", "Error", "Invalid Move");
        }

        public void DistributeMove(in int plrIndex, IMove move)
        {
            throw new System.NotImplementedException();
        }

        public async void AnnounceWinners(Dictionary<byte, GlobalPos> winners)
        {
            var title = "We have a winner!";
            if (winners.Count > 1)
            {
                title = "We have multiple winners!";
            }

            var names = "";
            foreach (var winner in winners)
            {
                if (names.Length > 0)
                {
                    names += '\n';
                }
                names += Game.GetPlayerList()[winner.Key].InGameName;
            }
            await _hubClients.Group(ID).SendAsync("ReceiveAlert", title, names);
            await _hubClients.Group(ID).SendAsync("DisplayWinningMoves", winners);
        }

        #endregion
        
    }
}