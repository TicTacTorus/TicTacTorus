using System;
using TicTacTorus.Source.Ingame.GridSpecificContent.Position;

namespace TicTacTorus.Source.Generator
{
    public class PositionGenerator : Randomizer
    {
        public PositionGenerator(int seed = DefaultSeed) : base(seed)
        {
        }

        public PositionGenerator(Random shared) : base(shared)
        {
        }
        
        public LocalPos GetLocalPos(int xMax = 0x100, int yMax = 0x100)
        {
            return new LocalPos((byte)_rnd.Next(xMax), (byte)_rnd.Next(yMax));
        }

        public GlobalPos GetGlobalPos(int xMax = int.MaxValue, int yMax = int.MaxValue)
        {
            return new GlobalPos(_rnd.Next(xMax), _rnd.Next(yMax));
        }
    }
}