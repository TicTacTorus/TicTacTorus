using System.Collections.Generic;

namespace TicTacTorus.Source
{
    public interface ILobby
    {
        public string Name { get; set; }
        public int MaxPlayerCount { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
        public bool IsPrivate { get; set; }

        public bool addPlayer(IPlayer player);
        public IList<IPlayer> GetAllPlayers();
    }
}