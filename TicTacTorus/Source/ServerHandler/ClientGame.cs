using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        [JsonIgnore] 
        private JsonSerializerSettings _jsonSerializerSettings;

        public ClientGame(Game game)
        {
            Game = game;
            Game.Parent = this;
            ID = game.ID.ToString();
            players = Game.GetPlayerList();
            PlayerOrder = game.PlayerOrder.ToArray();
            Settings = game.Settings;
            _jsonSerializerSettings = new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.All
            };
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

        public Tuple<bool, string, byte> SendMoveToGame(IMove move)
        {
            var (isValid, winners, nextPlayer) = Game.ReceivePlayerMove(move.Owner, move);
            if (!isValid)
            {
                return Tuple.Create<bool, string, byte>(false, null, nextPlayer);
            }

            if (winners == null)
            {
                return Tuple.Create<bool, string, byte>(true, null, nextPlayer);
            }

            var winnerMessage = GenerateWinnerMessage(winners);
            
            return Tuple.Create<bool, string, byte>(true, winnerMessage, nextPlayer);
        }

        private string GenerateWinnerMessage(IDictionary<byte, GlobalPos> winners)
        {
            var title = "We have a winner!\n";
            if (winners.Count > 1)
            {
                title = "We have multiple winners!\n";
            }

            var names = "";
            foreach (var winner in winners)
            {
                if (names.Length > 0)
                {
                    names += '\n';
                }
                // TODO: there should be no winner 255
                if (winner.Key < Game.GetPlayerList().Count)
                {
                    names += Game.GetPlayerList()[winner.Key].InGameName;
                }
            }

            return title + names;
        }
        
        /*
        public async void DenyMove(int plrIndex)
        {
            var group = Game.UniquePlayerGroup(Game.ID, plrIndex);
            await _hubClients.Group(group).SendAsync("ReceiveAlert", "Error", "Invalid Move");
        }

        public async Task DistributeMove(int plrIndex, IMove move)
        {
            //await _hubClients.Group(ID).SendAsync("ReceiveMessage", "Referee", "Invalid Move");
            var jsonMove = JsonConvert.SerializeObject(move, Formatting.Indented, _jsonSerializerSettings);
            await _hubClients.Group(ID).SendAsync("MoveFailed");
            await _hubClients.Group(ID).SendAsync("ReceivePlayerMove", jsonMove);
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
            await _hubClients.Group(ID).SendAsync("DisplayWinningMoves", JsonConvert.SerializeObject(winners, Formatting.Indented, _jsonSerializerSettings));
        }
        */
        #endregion
        
    }
}