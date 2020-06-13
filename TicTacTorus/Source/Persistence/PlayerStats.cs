using System.Collections.Generic;

namespace TicTacTorus.Source.Persistence
{
    public class PlayerStats: IPlayerStats
    {
        public int PlayedGames { get; set; }
        public int WonGames { get; set; }
        public List<int> Chains { get; set; }


        public PlayerStats() : this(0,0, new List<int>())
        {
            
        }
        public PlayerStats(int playedGames, int wonGames, List<int> chains)
        {
            PlayedGames = playedGames;
            WonGames = wonGames;
            Chains = chains;

        }
    }
}