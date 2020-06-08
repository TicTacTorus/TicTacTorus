namespace TicTacTorus.Source.Persistence
{
    public class PlayerStats: IPlayerStats
    {
        public int PlayedGames { get; set; }
        public int WonGames { get; set; }
        public int[] Chains { get; set; }


        public PlayerStats(int playedGames, int wonGames, int[] chains)
        {
            PlayedGames = playedGames;
            WonGames = wonGames;
            Chains = chains;

        }
    
    }
}