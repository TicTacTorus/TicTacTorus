using Blazor.Extensions;

namespace TicTacTorus.Source.Canvas
{
    public interface ICanvasRenderer
    {
        void Draw(BECanvasComponent canvas);
    }
}