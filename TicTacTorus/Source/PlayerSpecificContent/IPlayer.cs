using System.Drawing;
using System.Dynamic;

namespace TicTacTorus.Source
{
    public interface IPlayer
    {
        public string ID { get; set; }
        public string IngameName { get; set; }
        public Color Color { get; set; }
        public byte Symbol { get; set; }
    }
}