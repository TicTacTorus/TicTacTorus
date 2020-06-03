using System;
using System.Data.SQLite;
using System.Net.Mime;
using System.Numerics;
using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using TicTacTorus.Source.Ingame.GridSpecificContent.Chunk;
using TicTacTorus.Source.Ingame.GridSpecificContent.Position;

namespace TicTacTorus.Source.Canvas
{
    public class GameRenderer : ICanvasRenderer
    {
        private Grid _data;
        //private Image[] _symbols;

        private const int DefaultZoomSize = 32;
        //4th root of 2: zooming in 4 times doubles the zoom.
        private const double ZoomFactor = 1.18920711500272;
        private const double ZoomMax = 8.0;
        private const double ZoomMin = 1/ZoomMax;
        
        private double _zoom = 1;
        private Vector<double> _viewPoint = new Vector<double>(new double[] {0, 0});
        
        public GameRenderer(Grid grid /*, params Image[] symbols*/)
        {
            _data = grid;
        }

        public async void Draw(BECanvasComponent canvas)
        {
            var symbolSize = (int)(DefaultZoomSize * _zoom);

            var visible = new Vector<double>(new double[]{canvas.Width, canvas.Height}) * (DefaultZoomSize / _zoom);
            var center = _viewPoint * (DefaultZoomSize / _zoom);
            var begin = center + visible * 0.5;
            var end   = center - visible * 0.5;
            
            for (var y = (int) begin[1]; y < end[1]; ++y)
            {
                var drawY = GetTileDrawY(y);
                DrawLine(canvas, 0, drawY, (int)canvas.Height, drawY);
            }
            for (var x = (int) begin[0]; x < end[0]; ++x)
            {
                var deltaX = x - _viewPoint[0];
                var drawX = (int)(deltaX * symbolSize);
                DrawLine(canvas, drawX, 0, drawX, (int)canvas.Width);
            }
            
            GlobalPos gridPos = new GlobalPos();
            for (gridPos.Y = (int)begin[1]; gridPos.Y < end[1]; ++gridPos.Y)
            {
                //apparently vectors can't be constructed partially, since they have no this[] setters... bah!
                var drawY = GetTileDrawY(gridPos.Y);
                for (gridPos.X = (int)begin[0]; gridPos.X < end[0]; ++gridPos.X)
                {
                    var drawX = GetTileDrawX(gridPos.X);
                    var owner = _data.GetSymbol(gridPos);
                    DrawSymbol(canvas, drawX, drawY, symbolSize, symbolSize, owner);
                }
            }
        }

        private void DrawSymbol(BECanvasComponent canvas, int x, int y, int width, int height, byte owner)
        {
            //todo
            
        }

        private void DrawLine(BECanvasComponent canvas, int x1, int y1, int x2, int y2)
        {
            //todo
            //MoveTo(x1, y1);
            //LineTo(x2, y2);
        }

        int GetTileDrawX(int x)
        {
            var symbolSize = (int)(DefaultZoomSize * _zoom);
            var deltaX = x - _viewPoint[0];
            return (int)(deltaX * symbolSize);
        }
        
        int GetTileDrawY(int y)
        {
            var symbolSize = (int)(DefaultZoomSize * _zoom);
            var deltaX = y - _viewPoint[1];
            return (int)(deltaX * symbolSize);
        }

        public void MoveZoomedViewpoint(Vector<double> delta)
        {
            MoveViewpoint(delta * (1 / _zoom));
        }
        
        public void MoveViewpoint(Vector<double> delta)
        {
            _viewPoint += delta;
        }

        public void Zoom(int steps)
        {
            if (steps == 0)
            {
                return;
            }
            if (steps > 0)
            {
                ZoomIn(steps);
            }
            else
            {
                ZoomOut(-steps);
            }
        }
        
        public void ZoomIn(int steps = 1)
        {
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

        public void ZoomOut(int steps = 1)
        {
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
    }
}