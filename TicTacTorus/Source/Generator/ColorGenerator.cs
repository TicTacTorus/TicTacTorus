using System;
using System.Drawing;

namespace TicTacTorus.Source.Generator
{
    public class ColorGenerator
    {
        static Random rnd = new Random();

        public static Color GetColor()
        {
            Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            return randomColor;
        }
    }
}