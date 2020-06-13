using System;
using System.Collections.Generic;
using System.Data;
using TicTacTorus.Source.PlayerSpecificContent;
using System.Data.SQLite;
using System.Drawing;
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
			SQLiteConnection _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			//neuer Player wird bei Registrierung der Datenbank hinzugefügt
			if (createPlayer.ID != null ){
				_con.Open();
                                 
				SQLiteCommand command = new SQLiteCommand(_con);
				command.CommandText = "Insert Into User (loginName, salt, hash, inGameName, color, PlayerSymbol)" +
				                      "Values (@Id, @Salt, @Hash, @InGameName, @Color, @Symbol)";
				var IDParam = new SQLiteParameter("@Id", DbType.String, createPlayer.ID.Length) {Value = createPlayer.ID};
				var SaltParam = new SQLiteParameter("@Salt", DbType.Binary, SaltedHash.SaltBytes) {Value = createPlayer.Salt};
				var HashParam = new SQLiteParameter("@Hash", DbType.Binary, SaltedHash.HashBytes) {Value = createPlayer.Hash};
				var InGameNameParam = new SQLiteParameter("@InGameName", DbType.String, createPlayer.InGameName.Length) {Value = createPlayer.InGameName};
				var ColorParam = new SQLiteParameter("@Color", DbType.Int32, 4)
				{
					Value = createPlayer.PlrColor.R << 16 | createPlayer.PlrColor.G << 8 |
					        createPlayer.PlrColor.B | (0xFF - createPlayer.PlrColor.A) << 24
				};
				var SymbolParam = new SQLiteParameter("@Symbol", DbType.Byte, 1) {Value = createPlayer.Symbol};

				command.Parameters.Add(IDParam);
				command.Parameters.Add(SaltParam);
				command.Parameters.Add(HashParam);
				command.Parameters.Add(InGameNameParam);
				command.Parameters.Add(ColorParam);
				command.Parameters.Add(SymbolParam);
				
				command.Prepare();
				command.ExecuteNonQuery();
             
				//Player Statistic
			
				int[] ch = {0};
				
				
				PlayerStats playerStats = new PlayerStats(0,0,ch);
				createPlayer.playerStats = playerStats;
				PersistenceStorage.SavePlayerStat(createPlayer,playerStats);

				_con.Close();
				return true;
			}
			_con.Close();
			return false;
		}

		#endregion
		#region Load Methods

		//If other users want to look at your account site
		public static HumanPlayer LoadPlayer(string id)
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
				player.InGameName = reader[3] as string;
				//  player.Email = reader[4] as string;

				player.PlrColor = Color.FromArgb(Convert.ToInt32(reader[6]));
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
				player = new HumanPlayer();

				player.ID = reader[0] as string;
				player.Salt = reader[1] as byte[];
				player.Hash = reader[2] as byte[];
				player.InGameName = reader[3] as string;
				player.PlrColor = Color.FromArgb(Convert.ToInt32(reader[6]));
				//player.Symbol = (byte) reader[7];
				
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
			SQLiteConnection _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			_con.Open();
                                 
			SQLiteCommand command = new SQLiteCommand(_con);
            
			command.CommandText = $"select count(*) from User where loginName = '"+id+"';";
			
			if (Convert.ToInt32(command.ExecuteScalar()) > 0)
			{
				_con.Close();
				return false;
			}
			_con.Close(); 
			return true;
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

			command.CommandText = "Update User " +
			                      "SET salt = @Salt, " +
			                      "hash = @Hash " +
			                      "WHERE loginName = @Id";
			
			var IDParam = new SQLiteParameter("@Id", DbType.String, id.Length) {Value = id};                      
			var SaltParam = new SQLiteParameter("@Salt", DbType.Binary, SaltedHash.SaltBytes) {Value = newSalt};
			var HashParam = new SQLiteParameter("@Hash", DbType.Binary, SaltedHash.HashBytes) {Value = newHash};
			
			command.Parameters.Add(IDParam);
			command.Parameters.Add(SaltParam);
			command.Parameters.Add(HashParam);
			
			command.Prepare();
			command.ExecuteNonQuery();
			_con.Close(); 
		}
        /*   // not in use
		public static void UpdateEmail(string id, string email)
		{
			SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			_con.Open();
                                 
			SQLiteCommand command = new SQLiteCommand(_con);
            
			command.CommandText = $"update  User Set email ='"+ email+"' where loginName = '"+id+"'";
			command.ExecuteNonQuery();
			_con.Close(); 
		}
		*/
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
		#region Delete User
		public static void DeleteUser(string id)
		{
			SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			_con.Open();
                                 
			SQLiteCommand command = new SQLiteCommand(_con);
            
			command.CommandText = $"delete from User where loginName =" +id+"  ";
			command.ExecuteNonQuery();
			_con.Close();   

			DeletePlayerStat(id);
		}
		
		#endregion
		#region Symbol

		// Symbol, das hinter dem byte steht
		public static Blob GetSymbol(byte id)
		{
			var con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			con.Open();
			var command = new SQLiteCommand(con)
			{
				CommandText = $"select symbols from PlayerSymbols where PS ='" + id + "'"
			};

			var reader = command.ExecuteReader();
			Blob blob = new Blob();
			while (reader.Read())
			{
				blob = (Blob) reader[0] ;
			}
			con.Close();
			return blob;
		}

		// List von allen Symbolen (für Auswahl in Lobby/User)
		public static List<Blob> GetSymbols()
		{
			List<Blob> blobs = new List<Blob>();
			
			var con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			con.Open();
			var command = new SQLiteCommand(con)
			{
				CommandText = $"select symbols from PlayerSymbols"
			};

			var reader = command.ExecuteReader();
		
			while (reader.Read())
			{
				blobs.Add( (Blob) reader[0]) ;
			}
			con.Close();
			return blobs;
		}
		public static void SaveSymbol(Blob symbol)
		{
			SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			_con.Open();
                                 
			SQLiteCommand command = new SQLiteCommand(_con);
            
			command.CommandText = $"insert into PlayerSymbols(symbols) values ("+ symbol+") ";
			command.ExecuteNonQuery();
			_con.Close();   
		}
		public static void DeleteSymbol(byte id)
		{
			SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			_con.Open();
                                 
			SQLiteCommand command = new SQLiteCommand(_con);
            
			command.CommandText = $"delete from PlayerSymbols where ps =" +id+"  ";
			command.ExecuteNonQuery();
			_con.Close();   
		}
		
		#endregion
		#region AnonymNames

		public static string GetAnonymName(byte id)
		{
			string anonymName = "";
			var con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			con.Open();
			var command = new SQLiteCommand(con)
			{
				CommandText = $"select names from AnonymNames where AN ='" + id + "'"
			};

			var reader = command.ExecuteReader();
			
			while (reader.Read())
			{
				anonymName = (string) reader[0] ;
			}
			con.Close();
			return anonymName;
		}
		public static void SaveAnonymName(string anonymName)
		{
			SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			_con.Open();
                                 
			SQLiteCommand command = new SQLiteCommand(_con);
            
			command.CommandText = $"insert into AnonymNames(names) values ("+ anonymName+") ";
			command.ExecuteNonQuery();
			_con.Close();   

			
		}
		public static void DeleteAnonymName(byte id)
		{
			SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			_con.Open();
                                 
			SQLiteCommand command = new SQLiteCommand(_con);
            
			command.CommandText = $"delete from AnonymNames where AN =" +id+"  ";
			command.ExecuteNonQuery();
			_con.Close();   

			
		}
		
		#endregion
		#region PlayerStat
		public static PlayerStats GetPlayerStat(string id)
		{
			PlayerStats playerstat =null;
			
			var _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			_con.Open();
			SQLiteCommand command = new SQLiteCommand(_con);
		
			command.CommandText = $"select  p.PlayerName,p.playedGames,p.WonGames,c.Length,c.Value from PlayerStatistic p,Chains c " +
			                      $"where c.PlayerName = '"+ id +"' and c.PlayerName=p.PlayerName";
			
			var reader = command.ExecuteReader();
			var pg =0;
			var wg = 0;
			int[] ch = null;
			while (reader.Read())
			{ pg= (int) reader[1];
				 wg = (int) reader[2];
				
				ch[((int) reader[3]) - 1] = (int)reader[4];	
				playerstat = new PlayerStats(pg,wg,ch);
			}
			
			_con.Close();
			return playerstat;
		}

		public static void SavePlayerStat(HumanPlayer player, PlayerStats playStNewDif)
		{
			SQLiteConnection _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			_con.Open();
			
			SQLiteCommand command = new SQLiteCommand(_con);//for ExecuteReader
			SQLiteCommand command2 = new SQLiteCommand(_con);//foor ExecuteNonQuery
			command.CommandText = $"select count(*) from PlayerStatistic where PlayerName = '" + player.ID + "'  ";

			var reader = command.ExecuteReader();
			
			
			while (reader.Read())
				
			{var test = reader[0] is int ? (int) reader[0] : 0;
			
				
			if (test == 0)
			{
				//Playerstat doesen't exist with the loginName
				command2.CommandText = $"INSERT into PlayerStatistic (PlayerName, playedGames, WonGames) VALUES ('"+player.ID+"',"+playStNewDif.PlayedGames+","+playStNewDif.WonGames+")";
				
				command2.ExecuteNonQuery();

				for (var iter = 1; iter <= playStNewDif.Chains.Length; iter++)
				{
					command2.CommandText = $"INSERT INTO Chains(PlayerName, Length, Value) VALUES ('" + player.ID +
					                      "'," + iter + "," + playStNewDif.Chains[iter - 1] + ")";
					command2.ExecuteNonQuery();
				}

			}
			else
			{
				//Playerstat exists with loginName
				PlayerStats existPlayerStats;
				command.CommandText =
					$"select  p.PlayerName,p.playedGames,p.WonGames,c.Length,c.Value from PlayerStatistic p,Chains c " +
					$"where c.PlayerName = '" + player.ID + "' and c.PlayerName=p.PlayerName'";
				reader = command.ExecuteReader();
				var pg = 0;
				var wg = 0;
				int[] ch = null;
				while (reader.Read())
				{
					pg = (int) reader[1];
					wg = (int) reader[2];
					ch[((int) reader[3]) - 1] = (int) reader[4];

				}

				existPlayerStats = new PlayerStats(pg, wg, ch);
				existPlayerStats.PlayedGames += playStNewDif.PlayedGames;
				existPlayerStats.WonGames += playStNewDif.WonGames;
				command2.CommandText = $"update PlayerStatistic Set playedGames = " + existPlayerStats.PlayedGames +
				                      ",WonGames= " + existPlayerStats.WonGames + " where PlayerName = '" + player.ID +
				                      "'";
				command2.ExecuteNonQuery();

				//unterscheiden ob playStNewDif.Chains.Length länger als exist
				//Also ob neue Werte dazu kommen
				if (existPlayerStats.Chains.Length < playStNewDif.Chains.Length)
				{
					for (var iter = 0; iter < existPlayerStats.Chains.Length; iter++)
					{
						var iter2 = iter + 1;
						existPlayerStats.Chains[iter] += playStNewDif.Chains[iter];
						command2.CommandText = $"update Chains Set Value = " + existPlayerStats.Chains[iter] +
						                      " where PlayerName ='" + player.ID + "' and Length = " + iter2 + "";

						command2.ExecuteNonQuery();

					}

					for (var iter = existPlayerStats.Chains.Length; iter < playStNewDif.Chains.Length; iter++)
					{
						var iter2 = iter + 1;
						command2.CommandText = $"insert into Chains (PlayerName,Length,Value) values('" + player.ID +
						                      "'," + iter2 + "," + playStNewDif.Chains[iter] + ") ";
						command2.ExecuteNonQuery();
					}

				}
				else
				{
					for (var iter = 0; iter < playStNewDif.Chains.Length; iter++)
					{
						var iter2 = iter + 1;
						existPlayerStats.Chains[iter] += playStNewDif.Chains[iter];
						command2.CommandText = $"update Chains Set Value = " + existPlayerStats.Chains[iter] +
						                      " where PlayerName ='" + player.ID + "' and Length = " + iter2 + "";

						command2.ExecuteNonQuery();

					}
				}

			}

		}

		_con.Close();   

			
		}
		public static void DeletePlayerStat(string name)
		{
			SQLiteConnection  _con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
			_con.Open();
                                 
			SQLiteCommand command = new SQLiteCommand(_con);
            
			command.CommandText = $"delete from Chains where PlayerName = '" +name+" ' ";
			command.ExecuteNonQuery();
			command.CommandText = $"delete from PlayerStatistic where PlayerName = '" +name+" ' ";
			command.ExecuteNonQuery();
			_con.Close();   

			
		}
		#endregion 
	}
}