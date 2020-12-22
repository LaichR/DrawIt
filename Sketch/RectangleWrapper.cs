using Sketch.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sketch.Utilities
{
    public class RectangleWrapper: IBoundsProvider
    {
        Rect _r;
        public RectangleWrapper(Rect r)
        {
            _r = r;
        }
        public Rect Bounds => _r;
        
    }
}
