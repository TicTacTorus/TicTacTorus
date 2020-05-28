﻿using System;
using TicTacTorus.Source.Ingame.GridSpecificContent;
using TicTacTorus.Source.Ingame.GridSpecificContent.Chunk;
 using TicTacTorus.Source.Ingame.GridSpecificContent.Position;

 namespace TicTacTorus.Source.Ingame.GridSpecificContent.Chunk.Iterator
{
    /*
     Important notice: Do NOT keep an iterator for long.
     I can only guarantee the validity of the iterator as long as ReplaceMe() hasn't been called in both the current chunk as well as in the starting chunk.
     If the chunk replaces itself for the more space efficient version, the iterator can operate on an old chunk.
    */
    
    public class ChunkIterator
    {
        private BasicChunk _chunk;
        private readonly BasicChunk _startChunk;
        private LocalPos _pos;
        private readonly LocalPos _startPos;
        
        public ChunkIterator(BasicChunk chunk, LocalPos pos)
        {
            this._chunk = _startChunk = chunk;
            this._pos = _startPos = pos;
        }
        
        public byte Reset()
        {
            _chunk = _startChunk;
            _pos = _startPos;
            return Here();
        }
        
        public byte Here()
        {
            return _chunk.GetSymbol(_pos);
        }
        
        public byte Up()
        {
            if (_pos.Y > 0)
            {
                --_pos.Y;
            }
            else
            {
                _chunk = _chunk.Next.Up;
                _pos.Y = (byte)(_chunk.Height - 1);
            }
            return Here();
        }
        
        public byte UpRight()
        {
            Up();
            return Right();
        }
        
        public byte Right()
        {
            if (_pos.X < _chunk.Width - 1)
            {
                ++_pos.X;
            }
            else
            {
                _chunk = _chunk.Next.Right;
                _pos.X = 0;
            }
            return Here();
        }
        
        public byte DownRight()
        {
            Down();
            return Right();
        }
        
        public byte Down()
        {
            if (_pos.Y < _chunk.Height - 1)
            {
                ++_pos.Y;
            }
            else
            {
                _chunk = _chunk.Next.Down;
                _pos.Y = 0;
            }
            return Here();
        }
        
        public byte DownLeft()
        {
            Down();
            return Left();
        }
        
        public byte Left()
        {
            if (_pos.X > 0)
            {
                --_pos.X;
            }
            else
            {
                _chunk = _chunk.Next.Left;
                _pos.X = (byte)(_chunk.Width - 1);
            }
            return Here();
        }
        
        public byte UpLeft()
        {
            Up();
            return Left();
        }
    }
}