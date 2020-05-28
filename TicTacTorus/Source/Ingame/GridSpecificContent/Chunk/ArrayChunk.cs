using TicTacTorus.Source.Ingame.GridSpecificContent.Position;

namespace TicTacTorus.Source.Ingame.GridSpecificContent.Chunk
{
    public class ArrayChunk : BasicChunk
    {
        #region Fields
        
        public static int bitsPerEntry = 8;
        private byte[] _symbols;
        
        #endregion
        #region Constructors

        public ArrayChunk() : this(0, 0)
        {
        }
        
        public ArrayChunk(byte width, byte height) : base(width, height)
        {
            var size = Width * Height;
            _symbols = new byte[size];
            for (var i = 0; i < size; ++i)
            {
                _symbols[i] = 0xff;
            }
        }

        public ArrayChunk(LocalPos size) : this(size.X, size.Y)
        {
        }

        public ArrayChunk(BasicChunk init) : this((byte) init.Width, (byte) init.Height)
        {
            var index = 0;
            for (var y = 0; y < Height; ++y)
            {
                for (var x = 0; x < Width; ++x)
                {
                    _symbols[index++] = init.GetSymbol((byte) x, (byte) y);
                }
            }
            Next = init.Next;
        }

        #endregion
        #region Access / Functionality (Mostly Overloads)

        public override byte GetSymbol(byte x, byte y)
        {
            return _symbols[y * Width + x];
        }

        public override bool SetSymbol(byte x, byte y, byte owner, bool overwrite = false)
        {
            if (x >= Width || y >= Height)
            {
                return false;
            }

            var index = y * Width + x;
            if (!overwrite && _symbols[index] != BasicChunk.NoOwner)
            {
                return false;
            }
            
            _symbols[index] = owner;
            return true;
        }

        public override int GetCurrentSize()
        {
            return PredictStorageSize(0, Width, Height);
        }
        
        public override BasicChunk CreateReplacement()
        {
            //this is the final version, you won't replace me!
            return null;
        }
        
        #endregion
        #region Static Helper Functions

        public static int PredictStorageSize(int symbols, int width, int height)
        {
            return width * height * bitsPerEntry / 8;
        }

        #endregion
    }
}