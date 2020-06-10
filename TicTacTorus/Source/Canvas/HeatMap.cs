using System;
using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;
using TicTacTorus.Source.Ingame.GridSpecificContent.Position;

namespace TicTacTorus.Source.Canvas
{
    public class HeatMap : ICanvasRenderer
    {
        public byte[] Density { get; }
        
        public int Width { get; }
        public int Height { get; }
        private int _gridWidth;
        private int _gridHeight;

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
        
        public Task Draw(int width, int height, Canvas2DContext canvas)
        {
            var index = 0;
            for (var y = 0; y < Height; ++y)
            {
                for (var x = 0; x < Width; ++x, ++index)
                {
                    var val = Density[index];
                    if (val == 0)
                    {
                        continue;
                    }
                    canvas.SetFillStyleAsync(ChooseColor(val));
                    canvas.RectAsync(x, y, 1, 1);
                }
            }
            
            //ignore width and height, we have a fixed size. otherwise our data structure wouldn't make sense.
            throw new NotImplementedException();
        }

        private static string ChooseColor(byte density)
        {
            var part = ((byte)(0xff - density)).ToString("x2");
            return "#" + part + part + part;
        }
    }
}