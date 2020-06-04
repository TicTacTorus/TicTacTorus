using System.Drawing;
using Microsoft.AspNetCore.SignalR.Client;
using TicTacTorus.Source.LoginContent.Security;
using TicTacTorus.Source.PlayerSpecificContent;

namespace TicTacTorus.Source
{
    public class HumanPlayer : IPlayer
    {
        public string ID { get; set; }
        public string IngameName { get; set; }
        public Color Color { get; set; }
        public byte Symbol { get; set; }
        public byte[] Hash { get; set; } // Besprechen
        public byte[] Salt { get; set; }// Besprechen

        public HumanPlayer(string id, string ingameName, Color color, byte symbol)
        {
            ID = id;
            IngameName = ingameName;
            Color = color;
            Symbol = symbol;
        }
        public HumanPlayer(string id, string ingameName, Color color, byte symbol, string pwd):
            this(id, ingameName, color, symbol)
        {
            SaltedHash s = new SaltedHash(pwd); // Besprechen
            Hash = s.Hash;// Besprechen
            Salt = s.Salt;// Besprechen
        }
        
        public HumanPlayer(string ingameName, Color color, byte symbol):
            this(null, ingameName, color, symbol) { }

        public HumanPlayer()
        {
            ID = null;
        }
    }
}