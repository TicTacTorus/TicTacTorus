using System.Runtime.CompilerServices;

namespace TicTacTorus.Source.Ingame.GridSpecificContent.Position
{
    public struct LocalPos
    {
        public byte X { get; set; }
        public byte Y { get; set; }
        
        public LocalPos(byte x, byte y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ")";
        }
    }
}