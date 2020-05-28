using System.Collections.Generic;
using TicTacTorus.Source.Ingame.GridSpecificContent.Position;

 namespace TicTacTorus.Source.Ingame.GridSpecificContent.Chunk
{
    public class ListChunk : BasicChunk
    {
        #region Fields
        
        public static int bitsPerEntry = 8 * 11;
        private Dictionary<LocalPos, byte> _symbols = new Dictionary<LocalPos, byte>();
        
        #endregion
        #region Constructors
        
        public ListChunk() : this(0, 0)
        {
        }
        
        public ListChunk(byte width, byte height) : base(width, height)
        {
        }

        public ListChunk(LocalPos size) : this(size.X, size.Y)
        {
        }

        public ListChunk(BasicChunk init) : this((byte)init.Width, (byte)init.Height)
        {
            for (var y = 0; y < Height; ++y)
            {
                for (var x = 0; x < Width; ++x)
                {
                    var owner =  init.GetSymbol((byte)x, (byte)y);
                    if (owner != BasicChunk.NoOwner)
                    {
                        var pos = new LocalPos((byte)x, (byte)y);
                        _symbols[pos] = owner;
                    }
                }
            }
            Next = init.Next;
        }
        
        #endregion
        #region Access / Functionality (Mostly Overloads)
        
        public override byte GetSymbol(byte x, byte y)
        {
            if (x >= Width || y >= Height)
            {
                return BasicChunk.NoOwner;
            }
            
            var pos = new LocalPos(x, y);
            return !_symbols.ContainsKey(pos) ? BasicChunk.NoOwner : _symbols[pos];
        }

        public override bool SetSymbol(byte x, byte y, byte owner, bool overwrite = false)
        {
            if (x >= Width || y >= Height)
            {
                return false;
            }

            var pos = new LocalPos(x, y);
            if (!overwrite && _symbols.ContainsKey(pos))
            {
                return false;
            }
            
            _symbols[pos] = owner;
            return true;
        }

        public override int GetCurrentSize()
        {
            return PredictStorageSize(_symbols.Count, 0, 0);
        }

        public override BasicChunk CreateReplacement()
        {
            var listSize = PredictStorageSize(_symbols.Count, Width, Height);
            var arraySize = ArrayChunk.PredictStorageSize(_symbols.Count, Width, Height);
            if (listSize <= arraySize)
            {
                return null;
            }

            var replacement = new ArrayChunk(this);
            UpdateNeighbors(replacement);
            return replacement;
            
            //I have 2 approaches here: a dynamic (type based) one and a fast (hardcoded) one.
            //I decided to use the fast one above, for now. If we ever add more chunk types, I might maybe switch.
            //Since I like both fast code and dynamic code, I'm going to leave the other version here:
            
            /*
            var chunkTypes = new Type[] { typeof(ArrayChunk) };
            var mySize = GetStorageSize(_symbols.Count, Width, Height);
            var smallestType = ChooseSmallest(chunkTypes, _symbols.Count, Width, Height, mySize);
            if (smallestType == null)
            {
                return null;
            }
            var replacement = (BasicChunk) Activator.CreateInstance(smallestType, this);
            UpdateNeighbors(replacement);
            return replacement;
            */
        }
        
        #endregion
        #region Static Helper Functions
        
        public static int PredictStorageSize(int symbols, int width, int height)
        {
            //this chunk grows with (at least) one reference (8 bytes), one local pos (2 bytes) and the owner byte (1 byte) per move
            return symbols * bitsPerEntry;
        }

        #endregion
    }
}