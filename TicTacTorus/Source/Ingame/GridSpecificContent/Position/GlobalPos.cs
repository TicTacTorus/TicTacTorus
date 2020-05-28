namespace TicTacTorus.Source.Ingame.GridSpecificContent.Position
{
    public struct GlobalPos
    {
        public int X { get; set; }
        public int Y { get; set; }

        public GlobalPos(int x, int y)
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