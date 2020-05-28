using TicTacTorus.Source.Ingame.GridSpecificContent.Chunk.Iterator;
using TicTacTorus.Source.Ingame.GridSpecificContent.Position;

namespace TicTacTorus.Source.Ingame.GridSpecificContent.Grid
{
    public interface IGrid
    {
        public GlobalPos Size { get; }
        public int Width  => Size.X;
        public int Height => Size.X;
        
        byte GetSymbol(GlobalPos pos);
        bool SetSymbol(GlobalPos pos, byte owner, bool overwrite = false);
        public ChunkIterator GetIterator(GlobalPos pos);
    }
}