using System;
using System.Data.SQLite;
namespace TicTacTorus.Source.Persistence

{
    public class Database
    {
        protected internal static void Init()
        {
            SQLiteConnection con = new SQLiteConnection("Data Source=DatabaseTicTacTorus.dat");
            con.Open();
            
            SQLiteCommand command = new SQLiteCommand(con);
           
            command.CommandText = $"create table User (ID integer not null primary key autoincrement," +
                     "Name varchar(100)not null)";
            command.ExecuteNonQuery();
            con.Close();
        }
    }
}



