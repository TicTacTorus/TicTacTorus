using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;

namespace TicTacTorus.Source.Canvas
{
    public class MapRenderer : ICanvasRenderer
    {
        private HeatMap _data;

        public MapRenderer(HeatMap map)
        {
            _data = map;
        }

        public async Task Draw(int width, int height, Canvas2DContext canvas)
        {
            //todo: implement me!
        }
    }
}