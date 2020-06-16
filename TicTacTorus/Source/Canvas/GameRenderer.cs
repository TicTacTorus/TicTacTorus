using System;
using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
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
        private const double ZoomMin = 1 / ZoomMax;

        private double _zoom = 1;
        private double _viewX = 0, _viewY = 0;

        private double SymbolSize => DefaultZoomSize * _zoom;

        #endregion
        #region Constructors

        public GameRenderer(Grid grid, params (Bitmap, Color)[] symbols)
        {
            _data = grid;
            _viewX = (double)grid.Width / 2;
            _viewY = (double)grid.Height / 2;

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

            var visibleX = width / SymbolSize;
            var visibleY = height / SymbolSize;
            var top = _viewY - visibleY / 2 - 1;
            var bottom = _viewY + visibleY / 2;
            var left = _viewX - visibleX / 2 - 1;
            var right = _viewX + visibleX / 2;
            //Console.WriteLine($"GameRenderer::Draw values: SymbolSize = {SymbolSize:N2}, size = ({width:N2}, {height:N2}) -> ({visibleX:N2}, {visibleY:N2}), center = ({_viewX:N2}, {_viewY:N2}), top left = ({left:N2}, {top:N2}), bottom right = ({right:N2}, {bottom:N2})");

            await canvas.BeginBatchAsync();
            await canvas.BeginPathAsync();

            await DrawVisibleGrid(canvas, width, height, top, bottom, left, right);
            await DrawVisibleSymbols(canvas, width, height, top, bottom, left, right);
            
            await canvas.StrokeAsync();
            await canvas.EndBatchAsync();
        }

        private async Task DrawVisibleGrid(Canvas2DContext canvas, int width, int height, double top, double bottom, double left, double right)
        {
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
        }

        private async Task DrawVisibleSymbols(Canvas2DContext canvas, int width, int height, double top, double bottom, double left, double right)
        {
            var gridPos = new GlobalPos();
            for (gridPos.Y = (int) top; gridPos.Y < bottom; ++gridPos.Y)
            {
                var drawY = GetTileDrawY(gridPos.Y, height);
                for (gridPos.X = (int) left; gridPos.X < right; ++gridPos.X)
                {
                    var drawX = GetTileDrawX(gridPos.X, width);
                    var owner = _data.GetSymbol(gridPos);
                    await DrawSymbol(canvas, drawX, drawY, (int) SymbolSize, (int) SymbolSize, owner);
                }
            }
        }

        private async Task DrawSymbol(Canvas2DContext canvas, int x, int y, int width, int height, byte owner)
        {
            if (owner == BasicChunk.NoOwner)
            {
                return;
            }

            if (owner >= _symbols.Length)
            {
                await DrawDataSymbol(canvas, x, y, width, height, owner);
                return;
            }

            //todo: inform how to draw bitmaps, then draw _symbols[owner] here.
            //edit: okay, what the fuck is ElementReference!?
            //ElementReference image;
            //canvas.DrawImageAsync(image, x, y, width, height);
        }

        private static async Task DrawDataSymbol(Canvas2DContext canvas, int x, int y, int width, int height, byte owner)
        {
            //implemented before the bitmap drawing, but we can keep it in there, if for some reason there is no image.
            if (owner == BasicChunk.NoOwner)
            {
                return;
            }

            //circles and radial lines to easily distinguish the different values of the grid.
            const int radialLines = 5;
            const double border = 0.1;
            const double alpha = 1.0 / radialLines;
            var cx = x + width / 2;
            var cy = y + height / 2;
            var lineLength = (Math.Min(width, height) * (1 - border)) / 2;

            var lines = (owner) % radialLines;
            var circles = ((owner) / radialLines);

            for (var i = 0; i < lines + 1; ++i)
            {
                var angle = 2 * Math.PI * alpha * i;
                var dx = (int) (Math.Sin(angle) * lineLength);
                var dy = (int) (-Math.Cos(angle) * lineLength);
                await DrawLine(canvas, cx, cy, cx + dx, cy + dy);
            }

            if ((circles & 0x01) != 0)
            {
                await DrawPolygon(canvas, cx, cy, (int) lineLength / 2, 20);
            }
            if ((circles & 0x02) != 0)
            {
                await DrawPolygon(canvas, cx, cy, (int) lineLength, 20);
            }
        }

        private static async Task DrawLine(Canvas2DContext canvas, int x1, int y1, int x2, int y2)
        {
            //Console.WriteLine("GameRenderer::DrawLine(canvas, " + x1 + ", " + y1 + ", " + x2 + ", " + y2 + ")");
            await canvas.MoveToAsync(x1, y1);
            await canvas.LineToAsync(x2, y2);
        }

        private static async Task DrawPolygon(Canvas2DContext canvas, int x, int y, int r, int corners)
        {
            double PosX(double angle) => x + Math.Sin(angle) * r;
            double PosY(double angle) => y - Math.Cos(angle) * r;

            await canvas.MoveToAsync(PosX(0), PosY(0));
            for (var i = 1; i < corners; ++i)
            {
                var angle = i * Math.PI * 2 / corners;
                await canvas.LineToAsync(PosX(angle), PosY(angle));
            }
            await canvas.ClosePathAsync();
        }

        #endregion
        #region Viewpoint Movement / Zoom Logic

        private int GetTileDrawX(int x, int width = 0)
        {
            var delta = x - _viewX;
            return (int) (delta * SymbolSize) + width / 2;
        }

        private int GetTileDrawY(int y, int height = 0)
        {
            var delta = y - _viewY;
            return (int) (delta * SymbolSize) + height / 2;
        }

        public void MoveZoomedViewpoint(double dx, double dy)
        {
            MoveViewpoint(dx * (1 / SymbolSize), dy * (1 / SymbolSize));
        }

        private void MoveViewpoint(double dx, double dy)
        {
            _viewX += dx;
            _viewY += dy;

            _viewX %= _data.Width;
            _viewY %= _data.Height;
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

        private void ZoomIn(int steps = 1, double fixPointX = 0, double fixPointY = 0)
        {
            //var oldZoom = _zoom;
            for (var i = 0; i < steps; ++i)
            {
                _zoom *= ZoomFactor;
                if (_zoom > ZoomMax)
                {
                    _zoom = ZoomMax;
                    break;
                }
            }
        }

        private void ZoomOut(int steps = 1, double fixPointX = 0, double fixPointY = 0)
        {
            //var oldZoom = _zoom;
            for (var i = 0; i < steps; ++i)
            {
                _zoom /= ZoomFactor;
                if (_zoom < ZoomMin)
                {
                    _zoom = ZoomMin;
                    break;
                }
            }
        }
        
        public GlobalPos GetCursorPosition(double x, double y, double width, double height)
        {
            var trueX = _viewX + (x - width / 2) / SymbolSize;
            var trueY = _viewY + (y - height / 2) / SymbolSize;

            //int is rounding towards zero, so I have to add a distinguishing padding into the negative numbers
            if (trueX < 0)
            {
                --trueX;
            }
            if (trueY < 0)
            {
                --trueY;
            }

            return new GlobalPos((int) trueX, (int) trueY);
        }

        #endregion
    }
}