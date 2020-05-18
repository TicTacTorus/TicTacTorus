using System;
using Microsoft.VisualBasic.CompilerServices;

namespace TicTacTorus.Source.Generator
{
    public class ByteGenerator
    {
        private static Random rnd = new Random();

        public static byte GetByte()
        {
            return (byte) rnd.Next(byte.MaxValue);
        }
    }
}