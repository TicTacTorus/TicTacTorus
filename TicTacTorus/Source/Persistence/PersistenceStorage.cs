using TicTacTorus.Pages;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.Utility;
using System.Data.SQLite;
using System.Drawing;

using TicTacTorus.Source.LoginContent.Security;

namespace TicTacTorus.Source.Persistence
{
    public static class PersistenceStorage
    {             
        private static  SQLiteConnection  _con = new SQLiteConnection("Data Source=test.dat");
        #region Save Methods
 
        public static void SavePlayer(HumanPlayer savePlayer) // besprechen
        {
            
        }

        public static void CreatePlayer(HumanPlayer createPlayer) //NUR notNull Variable
        {//neuer Player wird bei Registrierung der Datenbank hinzugefügt
         if (createPlayer.ID != null ){
             _con.Open();
                                 
             SQLiteCommand command = new SQLiteCommand(_con);
            
             command.CommandText = $"insert into User (loginName,salt,hash,color,PlayerSymbol)values('"+ 
                                   createPlayer.ID  +" ','" +createPlayer.Salt+" ','" +
                                   createPlayer.Hash+" ','" +  
                                   createPlayer.Color+" ','" +createPlayer.Symbol +"')";
             command.ExecuteNonQuery();
             
             _con.Close();
         }
                       
        }
        public static void SaveGame(Game game)//besprechen
        {
            
        }

        public static void SavePlayerStats(IPlayerStats savePlayStats)//besprechen
        {
            
        }// PlayerStats implementiert?
	    
        #endregion
        #region Load Methods

        public static IPlayer LoadPlayer(string id)
        {  HumanPlayer player = new HumanPlayer();
            _con.Open();
                                 
            SQLiteCommand command = new SQLiteCommand(_con);
            
            command.CommandText = $"select * from User where loginName ='"+id+"'";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                player.ID = reader[0] as string;
                player.Salt =  reader[1] as byte[];
                player.Hash = reader[2] as byte[];
                player.IngameName = reader[3]as string;
              //  player.Email = reader[4] as string;
              //player.Pic = reader[5] as Image; //funktioniert nicht wegen image
          
              player.Color =Color.FromArgb((int) reader[6]);
              player.Symbol = (byte) reader[7] ;
            }
            _con.Close(); 
            return player;
        }

        public static IPlayer LoadPlayer(string id, string pw)
        {
            HumanPlayer player = new HumanPlayer();
            SaltedHash s = new SaltedHash(pw);

            _con.Open();
                                 
            SQLiteCommand command = new SQLiteCommand(_con);
            
            command.CommandText = $"select * from User where loginName ='"+id+
                                  "'and salt = '"+s.Salt+"'and hash ='"+s.Hash+"'";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                player.ID = reader[0] as string;
                player.Salt =  reader[1] as byte[];
                player.Hash = reader[2] as byte[];
                player.IngameName = reader[3]as string;
                //  player.Email = reader[4] as string;
                //player.Pic = reader[5] as Image; //funktioniert nicht wegen image
          
                player.Color =Color.FromArgb((int) reader[6]);
                player.Symbol = (byte) reader[7] ;
            }
            _con.Close(); 
            return player;
        }

        public static Replay LoadGame(Base64 base64)//Besprechen
        {
            return null;
        }

        public static IPlayerStats LoadPlayerStats(string loadPlayStats)//besprechen
        {
            return null;
        }

        #endregion
        #region Verify Methods

        //Checks if Password of userId is correct
        public static bool VerifyPassword(string id, string pw)
        {
            SaltedHash s =new SaltedHash(pw);
            var answer = false;
            _con.Open();
                                 
            SQLiteCommand command = new SQLiteCommand(_con);
            
            command.CommandText = $"select salt,hash from User where loginName ='"+id+"'";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (Equals(reader[0], s.Salt) && Equals(reader[1], s.Hash))
                {
                    answer = true;
                }
                
               
            }
            _con.Close(); 
            
            return answer;
        }
        //Checks if id is already taken
        public static bool CheckPlayerIdIsUnique(string id)
        {
            bool answer = false;
            _con.Open();
                                 
            SQLiteCommand command = new SQLiteCommand(_con);
            
            command.CommandText = $"select count(*) from User where loginName = '"+id+"';";
            var reader = command.ExecuteReader();
            if ((int) reader[0] == 1)
            {
                answer = true;
            }
            _con.Close(); 
            return answer;
       
        }

        #endregion
        #region Update Methods

        public static void UpdateInGameName(string id, string name)
        {
            _con.Open();
                                 
            SQLiteCommand command = new SQLiteCommand(_con);
            
            command.CommandText = $"update  User Set inGameName ='"+ name+"' where loginName = '"+id+"'";
            command.ExecuteNonQuery();
            _con.Close(); 
        }

        public static void UpdateSaltHash(string id,byte[]newSalt,byte[]newHash)
        {
            _con.Open();
                                 
            SQLiteCommand command = new SQLiteCommand(_con);
            
            command.CommandText = $"update  User Set salt ='"+ newSalt+"', hash ='"+ newHash+"' where loginName = '"+
            id+"'";
            command.ExecuteNonQuery();
            _con.Close(); 
        }
        
        public static void UpdateEmail(string id, string email)
        {
            _con.Open();
                                 
            SQLiteCommand command = new SQLiteCommand(_con);
            
            command.CommandText = $"update  User Set email ='"+ email+"' where loginName = '"+id+"'";
            command.ExecuteNonQuery();
            _con.Close(); 
        }
        
        public static void UpdateProfilePic(string id, string picPath)
        {
            // image.fromFile funktioniert nicht
        }
        public static void UpdateColor(string id, Color color)
        {
            _con.Open();
                                 
            SQLiteCommand command = new SQLiteCommand(_con);
            
            command.CommandText = $"update  User Set color ='"+ color.ToArgb()+"' where loginName = '"+id+"'";
            command.ExecuteNonQuery();
            _con.Close();   
        }
        public static void UpdatePlayerSymbol(string id, byte symbol)
        {
            _con.Open();
                                 
            SQLiteCommand command = new SQLiteCommand(_con);
            
            command.CommandText = $"update  User Set PlayerSymbol ='"+ symbol+"' where loginName = '"+id+"'";
            command.ExecuteNonQuery();
            _con.Close();   
        }
        #endregion
    }
}