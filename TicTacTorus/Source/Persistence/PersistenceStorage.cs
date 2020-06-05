﻿using System;
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
		// private static  SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
		#region Save Methods

		public static bool CreatePlayer(HumanPlayer createPlayer) //NUR notNull Variable
		{
			SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
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
			SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
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

				player.Color = Color.FromArgb(Convert.ToInt32(reader[6]));
				// player.Symbol = (byte) reader[7];
			}
			_con.Close();
			return player;
		}

		public static Tuple<HumanPlayer, bool> LoadPlayer(string id, string pw)
		{
			var con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			HumanPlayer player = null;
			SaltedHash sh = null;

			con.Open();

			var command = new SQLiteCommand(con)
			{
				CommandText = $"select * from User where loginName ='" + id + "'"
			};

			var reader = command.ExecuteReader();
			
			
			while (reader.Read())
			{
				player = new HumanPlayer
				{
					ID = reader[0] as string,
					Salt = reader[1] as byte[],
					Hash = reader[2] as byte[],
					IngameName = reader[3] as string,
					Color = Color.FromArgb((int) reader[6]),
					Symbol = (byte) reader[7]
				};
				sh = new SaltedHash(player.Salt, player.Hash);
				//player.Email = reader[4] as string;
				//player.Pic = reader[5] as Image; //funktioniert nicht wegen image
			}
			
			//Checks if Password is correct
			if (player != null && sh.Verify(pw))
			{
				con.Close();
				return Tuple.Create(player, true);
			}
			
			con.Close();
			return Tuple.Create(player, false);
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
			SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
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
			SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			_con.Open();
                                 
			SQLiteCommand command = new SQLiteCommand(_con);
            
			command.CommandText = $"select count(*) from User where loginName = '"+id+"';";
			
			if (Convert.ToInt32(command.ExecuteScalar()) > 0)
			{
				_con.Close(); 
				return true;
			}
			_con.Close(); 
			return false;
		}

		#endregion
		#region Update Methods

		public static void UpdateInGameName(string id, string name)
		{
			SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			_con.Open();
                                 
			SQLiteCommand command = new SQLiteCommand(_con);
            
			command.CommandText = $"update  User Set inGameName ='"+ name+"' where loginName = '"+id+"'";
			command.ExecuteNonQuery();
			_con.Close(); 
		}

		public static void UpdateSaltHash(string id,byte[]newSalt,byte[]newHash)
		{
			SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			_con.Open();
                                 
			SQLiteCommand command = new SQLiteCommand(_con);
            
			command.CommandText = $"update  User Set salt ='"+ newSalt+"', hash ='"+ newHash+"' where loginName = '"+
			                      id+"'";
			command.ExecuteNonQuery();
			_con.Close(); 
		}
        
		public static void UpdateEmail(string id, string email)
		{
			SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			_con.Open();
                                 
			SQLiteCommand command = new SQLiteCommand(_con);
            
			command.CommandText = $"update  User Set email ='"+ email+"' where loginName = '"+id+"'";
			command.ExecuteNonQuery();
			_con.Close(); 
		}
		
		public static void UpdateColor(string id, Color color)
		{
			SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			_con.Open();
                                 
			SQLiteCommand command = new SQLiteCommand(_con);
            
			command.CommandText = $"update  User Set color ='"+ color.ToArgb()+"' where loginName = '"+id+"'";
			command.ExecuteNonQuery();
			_con.Close();   
		}
		
		public static void UpdatePlayerSymbol(string id, byte symbol)
		{
			SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			_con.Open();
                                 
			SQLiteCommand command = new SQLiteCommand(_con);
            
			command.CommandText = $"update  User Set PlayerSymbol ='"+ symbol+"' where loginName = '"+id+"'";
			command.ExecuteNonQuery();
			_con.Close();   
		}
		#endregion
		#region Symbol

		// Symbol, das hinter dem byte steht
		public static Blob GetSymbol(byte id)
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