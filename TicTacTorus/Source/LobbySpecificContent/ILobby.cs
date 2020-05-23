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

        public bool AddPlayer(IPlayer player);
        public bool RemovePlayer(IPlayer player);
        public bool RemovePlayer(byte index);
        public IPlayer GetPlayerAt(byte index);
        public IList<IPlayer> GetAllPlayers();
    }
}