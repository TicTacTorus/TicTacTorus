using System.Drawing;

namespace TicTacTorus.Source
{
    public class HumanPlayer : IPlayer
    {
        public string ID { get; set; }
        public string IngameName { get; set; }
        public Color Color { get; set; }
        public byte Symbol { get; set; }

        public HumanPlayer(string id, string ingameName, Color color, byte symbol)
        {
            ID = id;
            IngameName = ingameName;
            Color = color;
            Symbol = symbol;
        }
    }
}