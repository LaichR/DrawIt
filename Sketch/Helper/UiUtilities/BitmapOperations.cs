using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Sketch.Helper.UiUtilities
{
    public static class BitmapOperations
    {
        public static Bitmap AddOverlay(Bitmap baseBmp, Bitmap overlayBmp)
        {
            Bitmap mergedBmp = new Bitmap(baseBmp);

            Graphics g = Graphics.FromImage(mergedBmp);
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            overlayBmp.MakeTransparent();
            int margin = 0;
            int x = mergedBmp.Width - overlayBmp.Width - margin;
            int y = mergedBmp.Height - overlayBmp.Height - margin;
            g.DrawImage(overlayBmp, new Point(x, y));
            return mergedBmp;
        }
    }
}
