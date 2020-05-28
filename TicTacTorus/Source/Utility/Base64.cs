using System;
using System.Linq;

namespace TicTacTorus.Source.Utility
{
    public class Base64
    {
        #region Fields

        private const int StandardLength = 8;
        private const int BitsPerDigit = 6;
        public const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
        public byte[] Data { get; }

        #endregion
        #region (Named) Constructors
        
        public Base64(byte[] init)
        {
            //make sure length is divisible by 3, add zero padding
            var padding = GetPadding(init.Length, 3);
            Data = new byte[init.Length + padding];
            init.CopyTo(Data, 0);
        }
        
        public Base64(string init)
        {
            //make sure length is divisible by 4, add zero padding
            init += new string(Alphabet[0], GetPadding(init.Length, 4));
            Data = new byte[(init.Length * BitsPerDigit)/8];

            for (var i = 0; i < init.Length / 4; ++i)
            {
                var segment = new char[4];
                for (var j = 0; j < segment.Length; ++j)
                {
                    var c = init[segment.Length * i + j];
                    if (!Alphabet.Contains(c))
                    {
                        throw new ArgumentOutOfRangeException("The character " + c + " is not a base 64 digit.");  
                    }
                    segment[j] = c;
                }
                
                Data[3 * i + 0] = (byte) (Alphabet.IndexOf(segment[0]) << 2 | Alphabet.IndexOf(segment[1]) >> 4);
                Data[3 * i + 1] = (byte) (Alphabet.IndexOf(segment[1]) << 4 | Alphabet.IndexOf(segment[2]) >> 2);
                Data[3 * i + 2] = (byte) (Alphabet.IndexOf(segment[2]) << 6 | Alphabet.IndexOf(segment[3]) >> 0);
            }
        }

        public static Base64 Random(int length = StandardLength)
        {
            var padding = GetPadding(length, 4);
            var data = new byte[(length + padding) * BitsPerDigit / 8];
            var rnd = new Random();
            for (var i = 0; i < data.Length; ++i)
            {
                data[i] = (byte)rnd.Next(0, 0x100);
            }
            return new Base64(data);
        }
        
        #endregion
        #region Helpers

        private static int GetPadding(int length, int segment)
        {
            return length % segment == 0 ? 0 : segment - (length % segment);
        }
        
        #endregion
        #region Operators

        static public bool operator ==(Base64 left, Base64 right)
        {
            var leftNull = object.ReferenceEquals(left, null);
            var rightNull = object.ReferenceEquals(right, null);
            //both null? equal
            if (leftNull && rightNull)
            {
                return true;
            }
            //one null? equaln't
            if (leftNull || rightNull)
            {
                return false;
            }
            //time to check the actual content
            return left.Data.SequenceEqual(right.Data);
        }
        
        static public bool operator !=(Base64 left, Base64 right)
        {
            return !(left == right);
        }
        
        #endregion
        #region String Representations

        public override string ToString()
        {
            var result = "";
            for (var i = 0; i < Data.Length / 3; ++i)
            {
                result += Alphabet[0                                 | Data[3 * i + 0] >> 2];
                result += Alphabet[(Data[3 * i + 0] & 0b000011) << 4 | Data[3 * i + 1] >> 4];
                result += Alphabet[(Data[3 * i + 1] & 0b001111) << 2 | Data[3 * i + 2] >> 6];
                result += Alphabet[(Data[3 * i + 2] & 0b111111) << 0 | 0                   ];
            }
            return result;
        }

        #endregion
    }
}