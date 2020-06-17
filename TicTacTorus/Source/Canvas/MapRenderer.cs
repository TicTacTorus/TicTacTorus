using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;

namespace TicTacTorus.Source.Canvas
{
    public class MapRenderer : ICanvasRenderer
    {
        private const double Darker = 0.7;

        private readonly HeatMap _data;
        private readonly GameRenderer _viewpoint;

        public MapRenderer(HeatMap map, GameRenderer view)
        {
            _data = map;
            _viewpoint = view;
        }

        public async Task Draw(int width, int height, Canvas2DContext canvas)
        {
            var index = 0;
            for (var y = 0; y < _data.Height; ++y)
            {
                for (var x = 0; x < _data.Width; ++x, ++index)
                {
                    var val = _data.Density[index];
                    if (val == 0)
                    {
                        continue;
                    }
                    
                    //todo: highlight the visible rectangle.
                    bool inside = true;

                    var clr = ChooseColor(val);
                    if (!inside)
                    {
                        DarkenColor(clr);
                    }
                    await canvas.SetFillStyleAsync(ColorToString(clr));
                    await canvas.RectAsync(x, y, 1, 1);
                }
            }
        }

        private void DarkenColor(Color clr)
        {
            clr = Color.FromArgb
            (
                clr.A,
                (byte)(clr.R * Darker),
                (byte)(clr.G * Darker),
                (byte)(clr.B * Darker)
            );
        }
        
        private static Color ChooseColor(byte density)
        {
            return Color.FromArgb
            (
                0xff,
                0xff,
                (byte)(0xff - density),
                (byte)(0xff - density)
            );
        }

        private static string ColorToString(Color clr)
        {
            return "#" + clr.R.ToString("x2") + clr.G.ToString("x2") + clr.B.ToString("x2");
        }
        
    }
}