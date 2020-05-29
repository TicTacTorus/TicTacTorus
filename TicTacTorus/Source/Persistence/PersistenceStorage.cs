using TicTacTorus.Pages;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Persistence
{
    public static class PersistenceStorage
    {
        #region Save Methods

        public static void SavePlayer(HumanPlayer savePlayer)
        {
            
        }

        public static void SaveGame(Game game)
        {
            
        }

        public static void SavePlayerStats(IPlayerStats savePlayStats)
        {
            
        }// PlayerStats implementiert?
	    
        #endregion
        #region Load Methods

        public static IPlayer LoadPlayer(string loadPlayer)
        {
            return null;
        }

        public static IPlayer LoadPlayer(string id, string pw)
        {
            return null;
        }

        public static Replay LoadGame(Base64 base64)
        {
            return null;
        }

        public static IPlayerStats LoadPlayerStats(string loadPlayStats)
        {
            return null;
        }

        #endregion
        #region Verify Methods

        //Checks if Password of userId is correct
        public static bool VerifyPassword(string id, string pw)
        {
            return false;
        }
        //Checks if id is already taken
        public static bool CheckPlayerIdIsUnique(string id)
        {
            return false;
        }

        #endregion
        #region Update Methods

        public static void UpdateUserName(string id, string name)
        {
            
        }

        public static void UpdateProfilePic(string id, string picPath)
        {
            
        }

        #endregion
    }
}