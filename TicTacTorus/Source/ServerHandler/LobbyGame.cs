using System.Collections.Generic;
using Newtonsoft.Json;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.PlayerSpecificContent;

namespace TicTacTorus.Source.ServerHandler
{
    public class LobbyGame
    {
        [JsonIgnore]
        private Game _game;

        public string ID { get; set; }
        public List<IPlayer> players { get; set; }

        public LobbyGame(Game game)
        {
            _game = game;
            ID = game.ID.ToString();
            players = _game.GetPlayerList();
        }

        public LobbyGame()
        {
            players = new List<IPlayer>();
        }

        #region Accessors

        public bool AddPlayer(IPlayer p)
        {
            if (!_game.AddPlayer(p)) return false;
            players.Add(p);
            return true;
        }

        #endregion
    }
}