using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;

namespace TicTacTorus.Source.Canvas
{
    public interface ICanvasRenderer
    {
        Task Draw(int width, int height, Canvas2DContext canvas);
    }
}