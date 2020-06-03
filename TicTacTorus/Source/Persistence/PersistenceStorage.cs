using System;
using System.Collections.Generic;
using TicTacTorus.Pages;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.Utility;
using System.Data.SQLite;
using System.Drawing;
using System.Net.Mime;
using System.Reflection.Metadata;
using TicTacTorus.Source.LoginContent.Security;

namespace TicTacTorus.Source.Persistence
{
	public static class PersistenceStorage
	{
		private static  SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
		#region Save Methods

		public static bool CreatePlayer(HumanPlayer createPlayer) //NUR notNull Variable
		{
			//neuer Player wird bei Registrierung der Datenbank hinzugefügt
			if (createPlayer.ID != null ){
				_con.Open();
                                 
				SQLiteCommand command = new SQLiteCommand(_con);
            
				command.CommandText = $"insert into User (loginName,salt,hash,color,PlayerSymbol)values('"+ 
				                      createPlayer.ID  +" ','" +createPlayer.Salt+" ','" +
				                      createPlayer.Hash+" ','" +  
				                      createPlayer.Color+" ','" +createPlayer.Symbol +"')";
				command.ExecuteNonQuery();
             
				_con.Close();
				return true;
			}

			return false;
		}
		
		public static void SavePlayerStats(IPlayerStats savePlayStats)
		{
            
		}
	    
		#endregion
		#region Load Methods

		//If other users want to look at your account site
		public static IPlayer LoadPlayer(string id)
		{  
			HumanPlayer player = new HumanPlayer();
			_con.Open();
                                 
			SQLiteCommand command = new SQLiteCommand(_con);
            
			command.CommandText = $"select * from User where loginName ='"+id+"'";
			var reader = command.ExecuteReader();
			while (reader.Read())
			{
				player.ID = reader[0] as string;
				//player.Salt =  reader[1] as byte[]; //remove
				//player.Hash = reader[2] as byte[]; //remove
				player.IngameName = reader[3] as string;
				//  player.Email = reader[4] as string;

				// player.Color = Color.FromArgb((int) reader[6]);
				// player.Symbol = (byte) reader[7];
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
			bool result = false;
			_con.Open();
                                 
			SQLiteCommand command = new SQLiteCommand(_con);
            
			command.CommandText = $"select count(*) from User where loginName = '"+id+"';";
			var reader = command.ExecuteReader();
			if (Convert.ToInt32(command.ExecuteScalar()) > 0)
			{
				return true;
			}
			_con.Close(); 
			return result;
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

		#region Symbol

		// Symbol, das hinter dem byte steht
		public static Blob GetSybol(byte id)
		{
			Blob blob = new Blob();
			return blob;
		}

		// List von allen Symbolen (für Auswahl in Lobby/User)
		public static List<Blob> GetSymbols()
		{
			List<Blob> blobs = new List<Blob>();
			return blobs;
		}

		#endregion
	}
}