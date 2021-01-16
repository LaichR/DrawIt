using Sketch.Interface;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sketch.Models
{
    class WaypointWrapper: IBoundsProvider
    {
        static readonly Size _size = new Size(0, 0);

        Rect r;
        RotateTransform _rotation = new RotateTransform();
        public WaypointWrapper(Point p)
        {
            r = new Rect(p, _size);
        }

        public System.Windows.Rect Bounds => r;

        public RotateTransform Rotation => _rotation;
        
    }
}
