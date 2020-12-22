using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Sketch.Models.Geometries
{
    public class Triangle: IConnectorEnding
    {
        static readonly PathSegmentCollection _segments = new PathSegmentCollection()
        {
            new LineSegment(new Point(0,0), true),
            new LineSegment( new Point(6,-14), true)
        };

        readonly PathFigure _arrowFigure = new PathFigure {
            StartPoint = new Point(-6, -14),
            Segments = _segments,
            IsClosed = true,
            IsFilled = true
        };

        
        
        double _rotation = 0;
        readonly double _myDefaultAngle = 90.0;
        double _scaleX = 1;
        double _scaleY = 1;
        Vector _translation = new Vector();
        Geometry _ending = null;

        

        public bool IsFilled
        {
            get
            {
                return _arrowFigure.IsFilled;
            }
            set
            {
                _arrowFigure.IsFilled = value;
            }
        }

        public Vector Translation
        {
            get
            {
                return _translation;
            }
            set
            {
                _translation = value;
                _ending = null;
            }
        }

        public double Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
                _ending = null;
            }
        }

        public double ScaleX
        {
            get
            {
                return _scaleX;
            }
            set
            {
                _scaleX = value;
                _ending = null;
            }
        }

        public double ScaleY
        {
            get
            {
                return _scaleY;
            }
            set
            {
                _scaleY = value;
                _ending = null;
            }
        }

        public Geometry Ending
        {
            get {
                if (_ending == null)
                {
                    ComputeGeometry();
                }
                return _ending;
            }

        }

        private void ComputeGeometry()
        {
           var t = new TransformGroup();
            var rotationAngle = _rotation - _myDefaultAngle;
           t.Children.Add(new ScaleTransform(_scaleX, _scaleY));
           t.Children.Add(new RotateTransform(rotationAngle,0,0));
           t.Children.Add(new TranslateTransform(_translation.X, _translation.Y)); 
           _ending = new PathGeometry(new[] { _arrowFigure }, FillRule.Nonzero, t);
        }
    }
}
