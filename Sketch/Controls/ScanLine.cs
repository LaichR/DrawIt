using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Sketch.Controls
{
    class ScanLine : IComparable
    {
        double _scanPos = 0;
        List<LineSegmentDecorator> _horizontalLines = new List<LineSegmentDecorator>();
        List<LineSegmentDecorator> _verticalLines = new List<LineSegmentDecorator>();
        ConnectorUI _connector;

        public ScanLine(double x)
        {
            _scanPos = x;
        }

        public ScanLine(LineSegmentDecorator s, double x)
        {
            if (s.IsHorizontal)
            {
                _horizontalLines.Add(s);
            }
            else
            {
                _verticalLines.Add(s);
            }
            
            _scanPos = x;
            _connector = s.Connector;
        }

        public ConnectorUI Connector
        {
            get
            {
                return _connector;
            }
        }

        public double ScanPos
        {
            get
            {
                return _scanPos;
            }
        }

        public int Count
        {
            get
            {
                return _verticalLines.Count + _horizontalLines.Count;
            }
        }

        public void Remove(LineSegmentDecorator l)
        {
            int count = Count;
            if (l.IsHorizontal)
            {
                _horizontalLines.Remove(l);
            }
            else
            {
                _verticalLines.Remove(l);
            }
            System.Diagnostics.Debug.Assert( Count < count);
        }

        public IEnumerable<LineSegmentDecorator> HorizontalLines
        {
            get
            {
                return _horizontalLines;
            }
        }

        public IEnumerable<LineSegmentDecorator> VertiacalLines
        {
            get
            {
                return _verticalLines;
            }
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            ScanLine other = obj as ScanLine;
            if (other == null) throw new ArgumentException();

            int comparison = _scanPos.CompareTo(other._scanPos);
            if (comparison == 0 && other.Count > 0)
            {
                _verticalLines.AddRange(other._verticalLines);
                _horizontalLines.AddRange(other._horizontalLines);
            }
            return comparison;
        }

#endregion
}
}
