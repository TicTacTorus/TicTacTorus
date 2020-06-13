using System.Collections.Generic;

namespace TicTacTorus.Source.Persistence
{
    public interface IPlayerStats
    {
        public int PlayedGames { get; set; }
        public int WonGames { get; set; }
        public List<int> Chains { get; set; }
    }
}