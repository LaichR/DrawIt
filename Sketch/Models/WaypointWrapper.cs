using Sketch.Interface;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sketch.Models
{
    class WaypointWrapper: IBoundsProvider
    {
        static readonly Size _size = new Size(0, 0);

        Rect r;
        public WaypointWrapper(Point p)
        {
            r = new Rect(p, _size);
        }

        public System.Windows.Rect Bounds => r;
    }
}
