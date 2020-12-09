using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using RuntimeCheck;
using Sketch.Interface;

namespace Sketch.Utilities
{
    public class BoundsComparer : IComparable
    {

        public enum CompareType
        {
            CompareX,
            CompareY
        };

        IBoundsProvider _boundedItem;
        CompareType _compareType = CompareType.CompareX;
        Func<object, int> _compareFunction;


        public BoundsComparer(IBoundsProvider item, CompareType compareType = CompareType.CompareX )
        {
            _compareType = compareType;
            _boundedItem = item;

            if (compareType == CompareType.CompareX)
            {
                _compareFunction = (x) => CompareX(x, true);
            }
            else
            {
                _compareFunction = (x) => CompareY(x, true);
            }
        }


        public bool Contains(Point p)
        {
            return _boundedItem.Bounds.Contains(p);
        }

        public IBoundsProvider Item
        {
            get 
            {
                return _boundedItem;
            }
        }

        public static implicit operator BoundsComparer(Rect r)
        {
            return new BoundsComparer(new RectangleWrapper(r));
        }

        public double Left => _boundedItem.Bounds.Left;

        public double Right => _boundedItem.Bounds.Right;

        public double Top => _boundedItem.Bounds.Top;

        public double Bottom => _boundedItem.Bounds.Bottom;

        public double Width => _boundedItem.Bounds.Width;

        public double Height => _boundedItem.Bounds.Height;


        public Point End
        {
            get;
            set;
        }

        public int CompareTo(object obj)
        {
            return _compareFunction(obj);
        }
        
        int CompareX(object obj, bool callCompareY )
        {
            var item = Assert.IsOfType<BoundsComparer>(obj);
            if (item.Right < Right) return 1;
            if (item.Right > Right) return -1;
            if (item.Width < Width) return 1;
            if (item.Width > Width) return -1;
            if (callCompareY) return CompareY(item, false);
            return 0;
        }

        int CompareY(object obj, bool callCompareX)
        {
            var item = Assert.IsOfType<BoundsComparer>(obj);
            if (item.Bottom < Bottom) return 1;
            if (item.Bottom > Bottom) return -1;
            if (item.Height < Height) return 1;
            if (item.Height > Height) return -1;
            if (callCompareX) return CompareX(item, false);
            return 0;
        }

    }
}
