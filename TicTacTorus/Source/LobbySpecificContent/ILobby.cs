using System.Collections.Generic;
using Base64 = TicTacTorus.Source.Utility.Base64;

namespace TicTacTorus.Source.LobbySpecificContent
{
    public interface ILobby
    {
        public string Name { get; set; }
        public int MaxPlayerCount { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public Base64 Id { get; set; }
        public bool IsPrivate { get; set; }

        public bool addPlayer(IPlayer player);
        public IList<IPlayer> GetAllPlayers();
    }
}