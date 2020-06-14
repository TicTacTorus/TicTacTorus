using System.Collections.Generic;
using Newtonsoft.Json;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.PlayerSpecificContent;

namespace TicTacTorus.Source.ServerHandler
{
    public class LobbyGame
    {
        [JsonIgnore]
        public Game Game { get; }

        public string ID { get; set; }
        public List<IPlayer> players { get; set; }

        public LobbyGame(Game game)
        {
            Game = game;
            ID = game.ID.ToString();
            players = Game.GetPlayerList();
        }

        public LobbyGame()
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
    }
}