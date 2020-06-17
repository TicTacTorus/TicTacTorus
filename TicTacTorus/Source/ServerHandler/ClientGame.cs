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

        public void SendMoveToGame(IMove move)
        {
            Game.ReceivePlayerMove(move.Owner, move);
        }

        public async void DenyMove(int plrIndex)
        {
            //await _hubClients.Group(ID).SendAsync("ReceiveMessage", "Referee", "Invalid Move");
            await _hubClients.Group(Game.UniquePlayerGroup(ID, plrIndex)).SendAsync( "MoveFailed");
        }

        public async Task DistributeMove(int plrIndex, IMove move)
        {
            //await _hubClients.Group(ID).SendAsync("ReceiveMessage", "Referee", "Invalid Move");
            var jsonMove = JsonConvert.SerializeObject(move, Formatting.Indented, _jsonSerializerSettings);
            await _hubClients.Group(ID).SendAsync("ReceivePlayerMove", jsonMove);
        }

        public async void AnnounceWinners(Dictionary<byte, GlobalPos> winners)
        {
            var jsonWinners = JsonConvert.SerializeObject(winners, Formatting.Indented, _jsonSerializerSettings);
            await _hubClients.Group(ID).SendAsync("AnnounceWinner", jsonWinners);
        }

        #endregion
        
    }
}