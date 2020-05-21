using System;

namespace TicTacTorus.Source.Utility
{
    public class Base64
    {
        private const int StandardLength = 8;
        private const int BitsPerDigit = 6;
        public const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
        public byte[] Data { get; }

        public Base64()
        {
            Data = new byte[StandardLength * BitsPerDigit / 8];

            var rnd = new Random();
            for (var i = 0; i < Data.Length; ++i)
            {
                Data[i] = (byte)rnd.Next(0, 0x100);
            }
        }
        
        public Base64(byte[] init)
        {
            //make sure length is divisible by 3, add zero padding
            var padding = 3 - (init.Length % 3);
            Data = new byte[init.Length + padding];
            init.CopyTo(Data, 0);
        }
        
        public Base64(string init)
        {
            //make sure length is divisible by 4, add zero padding
            if (init.Length % 4 > 0)
            {
                init += new string(Alphabet[0], 4 - (init.Length % 4));
            }
            
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
    }
}