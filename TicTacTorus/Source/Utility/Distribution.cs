using System;
using TicTacTorus.Source.Generator;

namespace TicTacTorus.Source.Utility
{
    /* Weighted Distribution Chooser */
    
    public class Distribution<T> : Randomizer
    {
        public (T, int)[] Data { get; }
        public int Sum { get; }

        public Distribution(Random shared, params (T, int)[] init) : base(shared)
        {
            Data = new (T, int)[init.Length];
            init.CopyTo(Data, 0);
            
            foreach(var entry in Data)
            {
                Sum += entry.Item2;
            }
        }
        public Distribution(int seed, params (T, int)[] init) : this(CreateRandom(seed), init)
        {
        }
        
        public Distribution(params (T, int)[] init) : this(DefaultSeed, init)
        {
        }
        
        public T Choose(Random rnd)
        {
            int goal = rnd.Next(Sum), progress = 0;
            for (var i = 0; i < Data.Length; ++i)
            {
                progress += Data[i].Item2;
                if (progress > goal)
                {
                    return Data[i].Item1;
                }
            }
            return Data[^1].Item1;
        }
        
        public T Choose()
        {
            return Choose(_rnd);
        }
    }
}