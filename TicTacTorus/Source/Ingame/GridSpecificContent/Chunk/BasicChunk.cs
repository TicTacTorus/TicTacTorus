using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Ingame.GridSpecificContent.Chunk
{
    public abstract class BasicChunk
    {
        public byte Width { get; set; }
        public byte Height { get; set; }

        private Navigation<BasicChunk> _chunks;

        /*public byte GetSymbol(LocalPos)
        {
            
        }*/
    }
}