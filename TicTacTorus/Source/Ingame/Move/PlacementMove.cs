using TicTacTorus.Source.Ingame.GridSpecificContent.Chunk;
using TicTacTorus.Source.Ingame.GridSpecificContent.Grid;
using TicTacTorus.Source.Ingame.GridSpecificContent.Position;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Ingame.Move
{
    public class PlacementMove : IMove
    {
        public byte Owner { set; get; }
        public GlobalPos Position { set; get; }

        public PlacementMove(byte who, GlobalPos where)
        {
            Owner = who;
            Position = where;
        }

        public bool CanDo(IGrid grid, Permutation playerOrder)
        {
            return grid.GetSymbol(Position) == BasicChunk.NoOwner;
        }
        
        public void Do(IGrid grid, Permutation playerOrder)
        {
            grid.SetSymbol(Position, Owner, true);
        }

        public void Undo(IGrid grid, Permutation playerOrder)
        {
            grid.SetSymbol(Position, BasicChunk.NoOwner, true);
        }

        public GlobalPos GetAreaCorner()
        {
            return Position;
        }

        public int GetAreaWidth()
        {
            return 1;
        }

        public int GetAreaHeight()
        {
            return 1;
        }
    }
}