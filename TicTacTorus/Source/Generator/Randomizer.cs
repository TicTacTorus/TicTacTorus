using System;
using System.Collections.Generic;

namespace TicTacTorus.Source.Generator
{
    /*
        abstract random generator base class.
        allows to have a own or shared random instance.
    */

    public abstract class Randomizer
    {
        protected const int DefaultSeed = -1;
        
        protected Random _rnd;

        protected Randomizer(int seed = DefaultSeed) : this(CreateRandom(seed))
        {
        }

        protected Randomizer(Random shared)
        {
            _rnd = shared;
        }
        
        public static Random CreateRandom(int seed = DefaultSeed)
        {
            return seed == DefaultSeed ? new Random() : new Random(seed);
        }

        public void ChangeSeed(int seed)
        {
            _rnd = CreateRandom(seed);
        }
    }
}