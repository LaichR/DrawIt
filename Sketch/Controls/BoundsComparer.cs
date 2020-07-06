using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Prism.Regions;
using RuntimeCheck;
using Sketch.Interface;

namespace Sketch.Controls
{
    class BoundsComparer : IComparable
    {

        public enum CompareType
        {
            CompareX,
            CompareY
        };

        IBoundedItemModel _boundedItem;
        CompareType _compareType = CompareType.CompareX;
        Func<object, int> _compareFunction;

        public BoundsComparer(IBoundedItemModel item, CompareType compareType = CompareType.CompareX )
        {
            _compareType = compareType;
            _boundedItem = item;

            if (compareType == CompareType.CompareX)
            {
                _compareFunction = (x) => CompareX(x);
            }
            else
            {
                _compareFunction = (x) => CompareY(x);
            }
        }

        public bool Contains(Point p)
        {
            return _boundedItem.Bounds.Contains(p);
        }

        public IBoundedItemModel Item
        {
            get 
            {
                return _boundedItem;
            }
        }

        public double Left => _boundedItem.Bounds.Left;

        public double Right => _boundedItem.Bounds.Right;
        
        public double Top => _boundedItem.Bounds.Top;

        public double Bottom => _boundedItem.Bounds.Bottom;


        public Point End
        {
            get;
            set;
        }

        public int CompareTo(object obj)
        {
            return _compareFunction(obj);
        }
        
        int CompareX(object obj)
        {
            var item = Assert.IsOfType<BoundsComparer>(obj);
            if (item.Left < Left) return -1;
            if (item.Left > Left) return 1;
            return 0;
        }

        int CompareY(object obj)
        {
            var item = Assert.IsOfType<BoundsComparer>(obj);
            if (item.Top < Top) return -1;
            if (item.Top > Top) return 1;
            return 0;
        }

    }
}
