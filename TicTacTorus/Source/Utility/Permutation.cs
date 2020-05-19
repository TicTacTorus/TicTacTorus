using System;

namespace TicTacTorus.Source.Utility
{
   public class IndexCollisionException : Exception
    {
        public IndexCollisionException()
        {
        }
        public IndexCollisionException(string message) : base(message)
        {
        }
        public IndexCollisionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
    
    public class Permutation
    {
        private int[] Items { get; }

        private int MinSize { get; }

        public Permutation(params int[] perm)
        {
            var size = perm.Length;
            var found = new bool[size];
            var min = 0;
            
            for (var i = 0; i < size; ++i)
            {
                var index = perm[i];
                //validate the correctness of a permutation.
                if (index >= size)
                {
                    throw new ArgumentOutOfRangeException("Permutation of size " + size + " is too small to contain " + index + ".");
                }
                if (found[index])
                {
                    throw new IndexCollisionException("Index collision detected: " + index + " appeared multiple times.");
                }
                //also keep track of the minimum size of this permutation
                if (index > i)
                {
                    min = index + 1;
                }
                found[index] = true;
            }

            MinSize = min;
            Items = new int[perm.Length];
            perm.CopyTo(Items, 0);
        }

        public Permutation(Permutation init)
        {
            //we can trust the data, it's already validated.
            MinSize = init.MinSize;
            Items = new int[init.Items.Length];
            init.Items.CopyTo(Items, 0);
        }
        
        public void IndexSwap(int a, int b)
        {
            (Items[a], Items[b]) = (Items[b], Items[a]);
        }
        
        public void Permute<T>(T[] list)
        {
            list = CreatePermutation(list);
        }

        public T[] CreatePermutation<T>(T[] list)
        {
            if (list.Length < MinSize)
            {
                throw new ArgumentOutOfRangeException("List is too short. This permutation needs a list of at least " + MinSize + " elements to stay in bounds.");
            }
            
            var result = new T[list.Length];
            for (var i = 0; i < Items.Length; ++i)
            {
                result[i] = list[Items[i]];
            }
            return result;
        }
    }
}