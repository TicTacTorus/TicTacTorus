using Blazor.Extensions;
using Blazor.Extensions.Canvas;

namespace TicTacTorus.Source.Canvas
{
    public class MapRenderer : ICanvasRenderer
    {
        private HeatMap _data;

        public MapRenderer(HeatMap map)
        {
            _data = map;
        }

        public async void Draw(BECanvasComponent canvas)
        {
            throw new System.NotImplementedException();
        }
    }
}