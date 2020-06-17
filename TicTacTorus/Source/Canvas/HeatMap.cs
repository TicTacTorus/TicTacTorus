using System;
using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;
using TicTacTorus.Source.Ingame.GridSpecificContent.Position;

namespace TicTacTorus.Source.Canvas
{
    public class HeatMap
    {
        public byte[] Density { get; }
        
        public int Width { get; }
        public int Height { get; }
        private readonly int _gridWidth;
        private readonly int _gridHeight;

        public HeatMap(int width, int height, int gridWidth, int gridHeight)
        {
            Width = width;
            Height = height;
            _gridWidth = gridWidth;
            _gridHeight = gridHeight;

            Density = new byte[width * height];
        }

        public void PlaceSymbol(GlobalPos pos)
        {
            var x = pos.X * Width / _gridWidth;
            var y = pos.Y * Height / _gridHeight;

            var index = y * Width + x;
            if (Density[index] < 0xff)
            {
                ++Density[index];
            }
        }

        public void FillArea(GlobalPos corner, int width, int height)
        {
            int top = corner.Y, bottom = corner.Y + height;
            int left = corner.X, right = corner.X + width;

            var pos = new GlobalPos(top, left);
            for (pos.Y = top; pos.Y < bottom; ++pos.Y)
            {
                for (pos.X = left; pos.X < right; ++pos.X)
                {
                    PlaceSymbol(pos);
                }
            }
        }
    }
}