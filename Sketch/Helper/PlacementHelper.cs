using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sketch.Helper
{
    /// <summary>
    /// Provides helper functions such as RoundToGrid
    /// </summary>
    public static class PlacementHelper
    {
        internal static double GridSize = 6.0;

        public static double RoundToGrid(double value)
        {
            return Math.Round(value / GridSize) * GridSize;
        }

        public static Point RoundToGrid(Point p)
        {
            var x = Math.Round(p.X / GridSize) * GridSize;
            var y = Math.Round(p.Y / GridSize) * GridSize;
            return new Point(x, y);
        }

        public static Size RoundToGrid(Size size)
        {
            var width = Math.Round(size.Width / GridSize) * GridSize;
            var height = Math.Round(size.Height / GridSize) * GridSize;
            return new Size(width, height);
        }

        public static Vector RoundToGrid(Vector v)
        {
            var x = Math.Round(v.X / GridSize) * GridSize;
            var y = Math.Round(v.Y / GridSize) * GridSize;
            return new Vector(x, y);
        }

        public static Rect RoundToGrid(Rect r)
        {
            return new Rect(RoundToGrid(r.Location), RoundToGrid(r.Size));
        }
    }
}
