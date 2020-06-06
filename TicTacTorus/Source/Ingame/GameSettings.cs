namespace TicTacTorus.Source.Ingame
{
	/*
        everything game relevant which comes from the lobby.
        (no, min/max player is specifically lobby related)
    */
	public struct GameSettings
	{
		public int TimeLimitSec;
		public int GridSize;
		public int WinChainLength;
	}
}