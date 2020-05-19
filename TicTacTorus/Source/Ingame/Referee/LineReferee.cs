using System;
using TicTacTorus.Source.GridSpecificContent.Chunk.Iterator;
using TicTacTorus.Source.Ingame.GridSpecificContent.Chunk;

namespace TicTacTorus.Source.Ingame.Referee
{
    public class LineReferee : IReferee
    {
        public int SymbolsNeeded { get; }

        public LineReferee(int length)
        {
            SymbolsNeeded = length;
        }
        
        public bool HasWon(Grid grid, GlobalPos pos)
        {
            var iterator = grid.GetIterator(pos);
            for (var i = 0; i < 4; ++i)
            {
                if (CheckDirection(iterator, i))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckDirection(ChunkIterator iterator, int direction)
        {
            var length = SegmentLength(iterator, direction, 1);
            if (length < SymbolsNeeded)
            {
                var opposite = (direction + 4) % 8;
                length = SegmentLength(iterator, opposite, length);
            }
            return length >= SymbolsNeeded;
        }

        private int SegmentLength(ChunkIterator iterator, int direction, int start)
        {
            var result = start;
            var owner = iterator.Here();
            for (var i = 0; i < SymbolsNeeded - start; ++i)
            {
                var symbol = direction switch
                {
                    0 => iterator.Up(),
                    1 => iterator.UpRight(),
                    2 => iterator.Right(),
                    3 => iterator.DownRight(),
                    4 => iterator.Down(),
                    5 => iterator.DownLeft(),
                    6 => iterator.Left(),
                    7 => iterator.UpLeft(),
                    _ => throw new ArgumentOutOfRangeException("internal error: direction does not exist")
                };
                if (iterator.Here() != owner || ++result >= SymbolsNeeded)
                {
                    break;
                }
            }
            iterator.Reset();
            return result;
        }
    }
}