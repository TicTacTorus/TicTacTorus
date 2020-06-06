using System;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mime;
using System.Numerics;
using System.Threading.Tasks;
//using System.Windows;
using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;
using TicTacTorus.Source.Ingame.GridSpecificContent.Chunk;
using TicTacTorus.Source.Ingame.GridSpecificContent.Position;

namespace TicTacTorus.Source.Canvas
{
    public class GameRenderer : ICanvasRenderer
    {
        #region Fields
        
        private Grid _data;
        private readonly Bitmap[] _symbols;

        private const int DefaultZoomSize = 100;
        //4th root of 2: zooming in 4 times doubles the zoom.
        private const double ZoomFactor = 1.18920711500272;
        private const double ZoomMax = 2;
        private const double ZoomMin = 1/ZoomMax;
        
        private double _zoom = 1;
        private double _viewX = 0, _viewY = 0;

        private double SymbolSize => DefaultZoomSize * _zoom;
        
        #endregion
        #region Constructors

        public GameRenderer(Grid grid, params (Bitmap, Color)[] symbols)
        {
            _data = grid;
            
            _symbols = new Bitmap[symbols.Length];
            for (var i = 0; i < _symbols.Length; ++i)
            {
                var modulation = symbols[i].Item2;
                var original = symbols[i].Item1;
                _symbols[i] = ImageManipulator.CreateColoredSymbol(original, modulation);
            }
        }

        #endregion
        #region Draw Methods
        
        public async Task Draw(int width, int height, Canvas2DContext canvas)
        {
            await canvas.BeginBatchAsync();
            await canvas.SetFillStyleAsync("White");
            await canvas.FillRectAsync(0, 0, width, height);
            
            //var symbolSize = DefaultZoomSize * _zoom;

            var visibleX = width  / SymbolSize;
            var visibleY = height / SymbolSize;
            var centerX = _viewX / SymbolSize;
            var centerY = _viewY / SymbolSize;
            var top    = centerY - visibleY / 2;
            var bottom = centerY + visibleY / 2;
            var left   = centerX - visibleX / 2;
            var right  = centerX + visibleX / 2;
            
            await canvas.BeginPathAsync();
            for (var y = (int) top; y < bottom; ++y)
            {
                //horizontal lines
                var drawY = GetTileDrawY(y, height);
                await DrawLine(canvas, 0, drawY, width, drawY);
            }
            for (var x = (int) left; x < right; ++x)
            {
                //vertical lines
                var drawX = GetTileDrawX(x, width);
                await DrawLine(canvas, drawX, 0, drawX, height);
            }
            
            var gridPos = new GlobalPos();
            for (gridPos.Y = (int)left; gridPos.Y < right; ++gridPos.Y)
            {
                var drawY = GetTileDrawY(gridPos.Y, height);
                for (gridPos.X = (int)left; gridPos.X < right; ++gridPos.X)
                {
                    var drawX = GetTileDrawX(gridPos.X, width);
                    var owner = _data.GetSymbol(gridPos);
                    await DrawSymbol(canvas, drawX, drawY, (int)SymbolSize, (int)SymbolSize, owner);
                }
            }
            await canvas.StrokeAsync();
            await canvas.EndBatchAsync();
        }

        private async Task DrawSymbol(Canvas2DContext canvas, int x, int y, int width, int height, byte owner)
        {
            if (owner == BasicChunk.NoOwner)
            {
                return;
            }
            
            
            
            Console.WriteLine("GameRenderer::DrawSymbol(canvas, " + x + ", " + y + ", " + width + ", " + height + ", 0x" + owner.ToString("x2") + ")");
            var border = 0.1;
            int x1 = (int)(x + width  * border), y1 = (int) (y + height * border);
            int w1 = (int) (width * (1 - 2 * border)), h1 = (int) (height * (1 - 2 * border));

            //await canvas.SetFillStyleAsync("black");
            //await canvas.FillRectAsync(x1, y1, w1, h1);
            
            await DrawLine(canvas, x1, y1, x1+w1, y1+h1);
            await DrawLine(canvas, x1, y1+h1, x1+w1, y1);
        }

        private async Task DrawLine(Canvas2DContext canvas, int x1, int y1, int x2, int y2)
        {
            //Console.WriteLine("GameRenderer::DrawLine(canvas, " + x1 + ", " + y1 + ", " + x2 + ", " + y2 + ")");
            await canvas.MoveToAsync(x1, y1);
            await canvas.LineToAsync(x2, y2);
        }

        #endregion
        #region Viewpoint Movement / Zoom Logic
        
        private int GetTileDrawX(int x, int width = 0)
        {
            var delta = x - _viewX;
            return (int)(delta * SymbolSize) + width/2;
        }

        private int GetTileDrawY(int y, int height = 0)
        {
            var delta = y - _viewY;
            return (int) (delta * SymbolSize) + height / 2;
        }

        public void MoveZoomedViewpoint(double dx, double dy)
        {
            MoveViewpoint(dx * (1/_zoom), dy * (1/_zoom));
        }
        
        public void MoveViewpoint(double dx, double dy)
        {
            _viewX += dx;
            _viewY += dy;
        }

        public void Zoom(int steps, double fixPointX = 0, double fixPointY = 0)
        {
            if (steps == 0)
            {
                return;
            }
            if (steps > 0)
            {
                ZoomIn(steps, fixPointX, fixPointY);
            }
            else
            {
                ZoomOut(-steps, fixPointX, fixPointY);
            }
        }
        
        public void ZoomIn(int steps = 1, double fixPointX = 0, double fixPointY = 0)
        {
            var oldZoom = _zoom;
            for (var i = 0; i < steps; ++i)
            {
                _zoom *= ZoomFactor;
                if (_zoom > ZoomMax)
                {
                    _zoom = ZoomMax;
                    break;
                }
            }

            AdjustViewpoint(oldZoom, fixPointX, fixPointY);
        }

        public void ZoomOut(int steps = 1, double fixPointX = 0, double fixPointY = 0)
        {
            var oldZoom = _zoom;
            for (var i = 0; i < steps; ++i)
            {
                _zoom /= ZoomFactor;
                if (_zoom < ZoomMin)
                {
                    _zoom = ZoomMin;
                    break;
                }
            }

            AdjustViewpoint(oldZoom, fixPointX, fixPointY);
        }

        private void AdjustViewpoint(double oldZoom, double fixPointX, double fixPointY)
        {
            var oldSize = DefaultZoomSize * oldZoom;
            var newSize = DefaultZoomSize * _zoom;
            
            //todo: future me, look further into this, it seems a bit fucky for now.
            
            var change = _zoom / oldZoom;
            var destX = _viewX + fixPointX;
            var destY = _viewY + fixPointY;
            
            _viewX += (destX - _viewX) * (1 - 1 / change);
            _viewY += (destY - _viewY) * (1 - 1 / change);
        }

        #endregion
    }
}