using Sketch.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Sketch.Helper
{
    public class RectangleWrapper: IBoundsProvider
    {
        Rect _r;

        RotateTransform _rotation;
        public RectangleWrapper(Rect r)
        {
            _r = r;
            _rotation = new RotateTransform();
        }

        public RectangleWrapper(Rect r, RotateTransform rotation)
        {
            _r = r;
            _rotation = rotation;
        }

        public Rect Bounds => _r;

        public RotateTransform Rotation => _rotation;
        
    }
}
