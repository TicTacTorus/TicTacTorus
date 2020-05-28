using System;

namespace TicTacTorus.Source.Utility
{
    #region Helpers
    
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

    #endregion
   
    public class Permutation
    {
        #region Fields

        private int[] _items;
        public int Length => _items.Length;
        public int this[int index] => index < Length ? _items[index] : index;
        public int MinSize { get; }

        #endregion
        #region (Named) Constructors
        
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
            _items = new int[perm.Length];
            perm.CopyTo(_items, 0);
        }

        public Permutation(Permutation init)
        {
            //we can trust the data, it's already validated.
            MinSize = init.MinSize;
            _items = new int[init._items.Length];
            init._items.CopyTo(_items, 0);
        }

        public static Permutation Ascending(int length)
        {
            var data = new int[length];
            for (var i = 0; i < length; ++i)
            {
                data[i] = i;
            }
            return new Permutation(data);
        }

        public static Permutation Descending(int length)
        {
            var data = new int[length];
            for (var i = 0; i < length; ++i)
            {
                data[i] = length - i - 1;
            }
            return new Permutation(data);
        }

        public static Permutation Random(int length)
        {
            var result = Ascending(length);
            result.ShuffleMe();
            return result;
        }

        public Permutation Inverse()
        {
            var data = new int[_items.Length];
            for (var i = 0; i < _items.Length; ++i)
            {
                //inverse of data[i] = Items[i];
                data[_items[i]] = i;
            }
            return new Permutation(data);
        }

        #endregion
        #region Permute Internally
        
        public void ShuffleMe()
        {
            var rnd = new Random();
            for (var i = 0; i < _items.Length; ++i)
            {
                IndexSwap(i, rnd.Next(_items.Length));
            }
        }
        
        public void IndexSwap(int a, int b)
        {
            (_items[a], _items[b]) = (_items[b], _items[a]);
        }
        
        #endregion
        #region Permute Externally
        
        public void Permute(Permutation target)
        {
            Permute(target._items);
        }

        public void Permute<T>(T[] list)
        {
            var result = ApplyPermutation(list);
            result.CopyTo(list, 0);
        }

        public T[] ApplyPermutation<T>(T[] list)
        {
            if (list.Length < MinSize)
            {
                throw new ArgumentOutOfRangeException("List is too short. This permutation needs a list of at least " + MinSize + " elements to stay in bounds.");
            }
            
            var result = new T[list.Length];
            for (var i = 0; i < list.Length; ++i)
            {
                //if a list is longer than the permutation, just interpret the missing part as Items[x] = x
                var index = i < _items.Length ? _items[i] : i;
                result[i] = list[index];
            }
            return result;
        }
        
        #endregion
        #region Mathematical Attributes

        public bool Parity()
        {
            //returns if the parity is an even amount of swaps away from the identity.

            //should be part of a good permutation class and could potentially be useful.
            throw new NotImplementedException("Permutation::Parity()");
        }

        public int Cycle()
        {
            //returns how often you have to apply the permutation until you reach the identity again.

            //should be part of a good permutation class and could potentially be useful.
            throw new NotImplementedException("Permutation::Cycle()");
        }
        
        #endregion
        #region String Representations

        public override string ToString()
        {
            var result = "";
            for (var i = 0; i < _items.Length; ++i)
            {
                if (i > 0)
                {
                    result += ", ";
                }
                result += _items[i];
            }
            return "{ " + result + " }";
        }

        #endregion
    }
}