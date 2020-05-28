using System;
using System.Collections.Generic;
using TicTacTorus.Source.Ingame.GridSpecificContent.Position;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Ingame.GridSpecificContent.Chunk
{
    public abstract class BasicChunk
    {
        #region Fields
        
        public const byte NoOwner = byte.MaxValue;
        public Navigation<BasicChunk> Next;
        private readonly LocalPos _size;
        public int Width => _size.X > 0 ? _size.X : 0x100;
        public int Height => _size.Y > 0 ? _size.Y : 0x100;

        #endregion
        #region Constructors
        
        protected BasicChunk() : this(0, 0)
        {
        }

        protected BasicChunk(byte width, byte height)
        {
            _size.X = width;
            _size.Y = height;
        }

        protected BasicChunk(LocalPos size)
        {
            _size = size;
        }

        #endregion
        #region Abstract Methods
        
        public abstract byte GetSymbol(byte x, byte y);
        public abstract bool SetSymbol(byte x, byte y, byte owner, bool overwrite = false);
        public abstract int GetCurrentSize();
        
        #endregion
        #region Access and Functionality

        public byte GetSymbol(LocalPos pos)
        {
            return GetSymbol(pos.X, pos.Y);
        }

        public bool SetSymbol(LocalPos pos, byte owner, bool overwrite = false)
        {
            return SetSymbol(pos.X, pos.Y, owner, overwrite);
        }

        public abstract BasicChunk CreateReplacement();

        public void UpdateNeighbors(BasicChunk newChunk)
        {
            if (Next.Up == this)
            {
                newChunk.Next.Up = newChunk;
            }
            else if(Next.Up != null)
            {
                Next.Up.Next.Down = newChunk;
            }

            if (Next.Down == this)
            {
                newChunk.Next.Down = newChunk;
            }
            else if (Next.Down != null)
            {
                Next.Down.Next.Up = newChunk;
            }

            if (Next.Left == this)
            {
                newChunk.Next.Left = newChunk;
            }
            else if (Next.Left != null)
            {
                Next.Left.Next.Right = newChunk;
            }

            if (Next.Right == this)
            {
                newChunk.Next.Right = newChunk;
            }
            else if (Next.Right != null)
            {
                Next.Right.Next.Left = newChunk;
            }
            
            //and detach myself
            Next.Up = Next.Down = Next.Left = Next.Right = null;
        }

        #endregion
        #region String Representations

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool verbose)
        {
            var result = "";
            for (var y = 0; y < Height; ++y)
            {
                if (y > 0)
                {
                    result += "\n";
                }
                for (var x = 0; x < Width; ++x)
                {
                    if (x > 0 && verbose)
                    {
                        result += " ";
                    }
                    var owner = GetSymbol((byte) x, (byte) y);
                    result += TileToString(owner);
                }
            }
            return result;
        }
        
        public static string TileToString(byte owner, bool verbose = false)
        {
            //note: the compact version only works (well) with 26 owners, the non-compact version is universal.
            return verbose
                ? owner == NoOwner ? "__" : owner.ToString("x2")
                : (owner == NoOwner ? '.' : (char) ('A' + owner)).ToString();
        }

        #endregion
        #region Static Reflection Methods
        
        public static Type ChooseSmallest(IEnumerable<Type> chunkTypes, int symbols, int width, int height, int maxSize = int.MaxValue)
        {
            //note: all the types need: public static int PredictStorageSize(int symbols, int width, int height)
            Type result = null;
            var smallest = maxSize;
            var parameters = new object[] { symbols, width, height };
            foreach (var type in chunkTypes)
            {
                var size = CallStorageSize(type, parameters);
                if (smallest > size)
                {
                    smallest = size;
                    result = type;
                }
            }
            return result;
        }

        public static int CallStorageSize(Type type, object[] parameters)
        {
            var method = type.GetMethod("PredictStorageSize");
            if (method != null)
            {
                return (int) method.Invoke(null, parameters);
            }
            return int.MaxValue;
        }
        #endregion
    }
}