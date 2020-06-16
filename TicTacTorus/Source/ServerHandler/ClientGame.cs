using System.Collections.Generic;
using Newtonsoft.Json;
using TicTacTorus.Source.Hubs;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.Ingame.GridSpecificContent.Position;
using TicTacTorus.Source.Ingame.Move;
using TicTacTorus.Source.PlayerSpecificContent;

namespace TicTacTorus.Source.ServerHandler
{
    public class ClientGame
    {
        [JsonIgnore]
        public Game Game { get; }

        public string ID { get; set; }
        public List<IPlayer> players { get; set; }
        public GameSettings Settings { get; set; }

        public ClientGame(Game game)
        {
            Game = game;
            Game.Parent = this;
            ID = game.ID.ToString();
            players = Game.GetPlayerList();
            Settings = game.Settings;
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

        public void DenyMove(in int plrIndex)
        {
            throw new System.NotImplementedException();
        }

        public void DistributeMove(in int plrIndex, IMove move)
        {
            throw new System.NotImplementedException();
        }

        public void AnnounceWinners(Dictionary<byte, GlobalPos> winners)
        {
            throw new System.NotImplementedException();
        }
    }
}