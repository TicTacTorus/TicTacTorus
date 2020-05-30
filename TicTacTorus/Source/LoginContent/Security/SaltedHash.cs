using System;
using System.Linq;
using System.Security.Cryptography;

namespace TicTacTorus.Source.LoginContent.Security
{
    public class SaltedHash
    {
        public const int SaltBytes = 64;

        public byte[] Salt { get; }
        public byte[] Hash { get; }
        private static readonly SHA256 Generator = SHA256.Create();

        public SaltedHash(string plainText)
        {
            var rnd = new Random();

            Salt = new byte[SaltBytes];
            for (int i = 0; i < SaltBytes; ++i)
            {
                Salt[i] = (byte) rnd.Next(0, 0x100);
            }

            var plainBytes = Str2Bin(plainText);
            var input = Concat(Salt, plainBytes);
            Hash = Generator.ComputeHash(input);
        }
        
        public SaltedHash(byte[] salt, byte[] hash)
        {
            this.Salt = salt;
            this.Hash = hash;
        }

        public bool Verify(string plainText)
        {
            var plainBytes = Str2Bin(plainText);
            var input = Concat(Salt, plainBytes);
            var hashed = Generator.ComputeHash(input);
            return hashed.SequenceEqual(Hash);
        }

        private byte[] Str2Bin(string plainText)
        {
            var result = new byte[sizeof(char)*plainText.Length];
            int index = 0;
            foreach (var chr in plainText)
            {
                for (int i = 0; i < sizeof(char); ++i)
                {
                    result[index++] = (byte)(chr >> 8 * i & 0xff);
                }
            }
            return result;
        }
        
        private byte[] Concat(params byte[][] arrays)
        {
            var length = arrays.Sum(arr => arr.Length);
            var result = new byte[length];
            var index = 0;
            foreach (var arr in arrays)
            {
                arr.CopyTo(result, index);
                index += arr.Length;
            }
            return result;
        }
    }
}