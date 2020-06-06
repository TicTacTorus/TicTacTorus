using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace TicTacTorus.Source.Canvas
{
    public static class ImageManipulator
    {
        public static Bitmap ApplyColorFunction(Bitmap img, Func<Color, Color> function)
        {
            var result = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
            for (var y = 0; y < img.Height; ++y)
            {
                for (var x = 0; x < img.Width; ++x)
                {
                    //this screams for a faster solution. GetPixel() and SetPixel() usually add an unsatisfactory amount of overhead...
                    //I want my pointers for direct iteration back, or at least some writeable iterators... ;_;
                    result.SetPixel(x, y, function(img.GetPixel(x, y)));
                }
            }            
            return result;
        }

        public static Bitmap CreateColoredSymbol(Bitmap img, Color mod, bool darkMode = false)
        {
            if (!darkMode)
            {
                return ApplyColorFunction(img, (clr) => Overlay(ModulateWhite(clr, mod), Color.White));
            }

            return ApplyColorFunction(img, (clr) =>
            {
                var inverse = Color.FromArgb(clr.A, 0xff - clr.R, 0xff - clr.G, 0xff - clr.B);
                return Overlay(ModulateBlack(inverse, mod), Color.Black);
            });
        }

        public static Color ModulateColor(Color clr, Color mod, Func<byte, byte, byte> rgb, Func<byte, byte, byte> a = null)
        {
            a ??= rgb;
            return Color.FromArgb(a(clr.A, mod.A), rgb(clr.R, mod.R), rgb(clr.G, mod.G), rgb(clr.B, mod.B));
        }

        public static Color ModulateWhite(Color clr, Color mod)
        {
            //white -> mod, black -> black
            return ModulateColor(clr, mod,
                (rgba, m) => (byte) (rgba * m / 0xff));
        }

        public static Color ModulateBlack(Color clr, Color mod)
        {
            //white -> white, black -> mod
            return ModulateColor(clr, mod, 
                (rgb, m) => (byte)(0xff - (0xff - rgb) * (0xff - m) / 0xff),
                (a, m) => (byte)(a * m / 0xff));
        }

        public static Color Overlay(Color src, Color dst)
        {
            //draws source over destination
            if (src.A == 0 && dst.A == 0)
            {
                return Color.Transparent;
            }
            if (src.A == 0xff)
            {
                return src;
            }

            // c1 * a1 + c2 * a2 * (1-a1) / (a1 + a2 * (1 - a1))
            
            var srcA = (float)src.A / 0xff;
            var dstA = (float)dst.A / 0xff;
            var denominator = (srcA + dstA * (1 - srcA));
            var alpha = (byte) (0xff * (1 - (1 - dstA) * (1 - srcA)));
            
            return ModulateColor(src, dst, 
                (rgb1, rgb2) => (byte)(rgb1 * srcA + rgb2 * dstA * (1 - srcA) / denominator),
                (a1, a2) => alpha);
        }

    }
}