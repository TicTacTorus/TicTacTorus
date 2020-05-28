using System.Drawing;
using Microsoft.AspNetCore.SignalR.Client;
using TicTacTorus.Source.PlayerSpecificContent;

namespace TicTacTorus.Source
{
    public class HumanPlayer : IPlayer
    {
        public string ID { get; set; }
        public string IngameName { get; set; }
        public Color Color { get; set; }
        public byte Symbol { get; set; }
        public HubConnection Connection { get; set; }

        public HumanPlayer(string id, string ingameName, Color color, byte symbol)
        {
            ID = id;
            IngameName = ingameName;
            Color = color;
            Symbol = symbol;
        }

        public HumanPlayer()
        {
            
        }
    }
}