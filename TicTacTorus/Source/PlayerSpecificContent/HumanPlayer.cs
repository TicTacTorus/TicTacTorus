using System.Drawing;
using Microsoft.AspNetCore.SignalR;
using TicTacTorus.Source.Hubs;
using TicTacTorus.Source.Ingame.GridSpecificContent.Grid;
using TicTacTorus.Source.Ingame.Move;
using TicTacTorus.Source.LoginContent.Security;
using TicTacTorus.Source.Persistence;

namespace TicTacTorus.Source.PlayerSpecificContent
{
    public class HumanPlayer : IPlayer
    {
        public string ID { get; set; }
        public string SessionID { get; set; }
        public string InGameName { get; set; }
        public Color PlrColor { get; set; }
        public byte Symbol { get; set; }
        public byte[] Hash { get; set; } 
        public byte[] Salt { get; set; }
        public PlayerStats playerStats { get; set; }//Besprechen
        
        public HumanPlayer(string id, string inGameName, Color plrColor, byte symbol)
        {
            ID = id;
            InGameName = inGameName;
            PlrColor = plrColor;
            Symbol = symbol;
            playerStats = null;
        }
        public HumanPlayer(string id, string inGameName, Color plrColor, byte symbol, string pwd):
            this(id, inGameName, plrColor, symbol)
        {
            SaltedHash s = new SaltedHash(pwd); // Besprechen
            Hash = s.Hash;// Besprechen
            Salt = s.Salt;// Besprechen
        }
      //playerstats insert
    
        
        public HumanPlayer(string ingameName, Color color, byte symbol):
            this(null, ingameName, color, symbol) { }

        public HumanPlayer()
        {
            ID = null;
        }
        
        public IMove ChooseMove(IClientProxy connection, IGrid grid, int moveSeconds)
        {
            connection.SendAsync("ActivatePlayer");

            //todo: wait, receive
            
            
            throw new System.NotImplementedException();
        }

    }
}