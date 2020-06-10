﻿using System;
using TicTacTorus.Source.Ingame.GridSpecificContent.Chunk.Iterator;
 using TicTacTorus.Source.Ingame.GridSpecificContent.Grid;
 using TicTacTorus.Source.Ingame.GridSpecificContent.Position;

 namespace TicTacTorus.Source.Ingame.GridSpecificContent.Chunk
{
    public class Grid : IGrid
    {
        public GlobalPos Size { get; }
        public int Width  => Size.X;
        public int Height => Size.Y;

        private BasicChunk[] _chunks;
        private readonly GlobalPos _chunkCount;

        private readonly bool _autoReplace;
        
        public Grid(int width, int height, bool replaceChunks = false) : this(new GlobalPos(width, height), replaceChunks)
        {
        }

        public Grid(GlobalPos pos, bool replaceChunks = false)
        {
            Size = pos;
            _chunkCount = new GlobalPos
            (
                SlotsNeeded(Width , 0x100),
                SlotsNeeded(Height, 0x100)
            );
            _autoReplace = replaceChunks;
            CreateChunks();
            LinkChunks();
        }

        private void CreateChunks()
        {
            var rest = unchecked(new LocalPos((byte)Width, (byte)Height));

            //regular chunk size, border right size, border bottom size, corner bottom right size
            var sizes = unchecked(new LocalPos[]
            {
                new LocalPos(0, 0),
                new LocalPos(rest.X, 0),
                new LocalPos(0, rest.Y),
                rest
            });
            
            _chunks = new BasicChunk[_chunkCount.X * _chunkCount.Y];
            var index = 0;
            for (var y = 0; y < _chunkCount.Y; ++y)
            {
                var lastY = y == _chunkCount.Y - 1;
                for (var x = 0; x < _chunkCount.X; ++x, ++index)
                {
                    var lastX = x == _chunkCount.X - 1;
                    var chunkSize = sizes[(lastX ? 1 : 0) + (lastY ? 2 : 0)];
                    _chunks[index] = new ListChunk(chunkSize);
                }
            }
        }
        
        private void LinkChunks()
        {
            for (var y = 0; y < _chunkCount.Y; ++y)
            {
                for (var x = 0; x < _chunkCount.X; ++x)
                {
                    var here = _chunks[ChunkIndex(x, y)];
                    here.Next.Up    = _chunks[ChunkIndex(x+0, y-1)];
                    here.Next.Right = _chunks[ChunkIndex(x+1, y+0)];
                    here.Next.Down  = _chunks[ChunkIndex(x+0, y+1)];
                    here.Next.Left  = _chunks[ChunkIndex(x-1, y+0)];
                }
            }
        }

        private static int SlotsNeeded(int whole, int part)
        {
            //basically integer division but rounding up to accomodate for partial segments
            return whole / part + (whole % part > 0 ? 1 : 0);
        }
        
        private int ChunkIndex(int x, int y)
        {
            return PositiveMod(y, _chunkCount.Y) * _chunkCount.X + PositiveMod(x, _chunkCount.X);
        }

        private static int PositiveMod(int x, int m)
        {
            //force x always into [0, m), even if x or m is negative
            x %= Math.Abs(m);
            return x < 0 ? x + Math.Abs(m) : x;
        }
        
        private BasicChunk GetChunkAt(GlobalPos pos)
        {
            var x = pos.X >> 8;
            var y = pos.Y >> 8;
            return _chunks[ChunkIndex(x, y)];
        }

        private void SetChunkAt(GlobalPos pos, BasicChunk newChunk)
        {
            var x = pos.X >> 8;
            var y = pos.Y >> 8;
            var oldChunk = _chunks[ChunkIndex(x, y)];
            oldChunk.UpdateNeighbors(newChunk);
            _chunks[ChunkIndex(x, y)] = newChunk;
        }
        
        public byte GetSymbol(GlobalPos pos)
        {
            //automatic wraparound for easier usage.
            var inside = new GlobalPos(PositiveMod(pos.X, Width), PositiveMod(pos.Y, Height));
            
            var local = unchecked(new LocalPos((byte)inside.X, (byte)inside.Y));
            var chunk = GetChunkAt(inside);
            return chunk.GetSymbol(local);
        }

        public bool SetSymbol(GlobalPos pos, byte owner, bool overwrite = false)
        {
            //automatic wraparound for easier usage.
            var inside = new GlobalPos(PositiveMod(pos.X, Width), PositiveMod(pos.Y, Height));
            
            var local = unchecked(new LocalPos((byte)inside.X, (byte)inside.Y));
            var chunk = GetChunkAt(inside);
            var result = chunk.SetSymbol(local, owner, overwrite);
            if (_autoReplace)
            {
                var replacement = chunk.CreateReplacement();
                if (replacement != null)
                {
                    SetChunkAt(inside, replacement);
                }
            }
            return result;
        }

        public ChunkIterator GetIterator(GlobalPos pos)
        {
            var local = unchecked(new LocalPos((byte)pos.X, (byte)pos.Y));
            return new ChunkIterator(GetChunkAt(pos), local);
        }

        public override string ToString()
        {
            return "Grid: "
               + Size.X + "(" + _chunkCount.X + ") x "
               + Size.Y + "(" + _chunkCount.Y + ")";
        }
    }
}