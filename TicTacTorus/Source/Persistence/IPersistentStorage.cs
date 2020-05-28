using System;
using TicTacTorus.Pages;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Persistence
{
	public interface IPersistentStorage
	{
		#region Save Methods
	    
		void SavePlayer(HumanPlayer savePlayer);
		void SaveGame(Game game);
		void SavePlayerStats(IPlayerStats savePlayStats);// PlayerStats implementiert?
	    
		#endregion
		#region Load Methods

		IPlayer LoadPlayer(string loadPlayer);
		IPlayer LoadPlayer(string id, string pw);
		Replay LoadGame(Base64 base64);
		IPlayerStats LoadPlayerStats(string loadPlayStats);

		#endregion
		#region Verify Methods

		//Checks if Password of userId is correct
		bool VerifyPassword(string id, string pw);
		//Checks if id is already taken
		bool CheckPlayerIdIsUnique(string id);

		#endregion   
	}
}