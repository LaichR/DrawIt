using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Sketch.Controls
{
    class LineSegmentDecorator
    {
        int _id;

        ConnectorUI _connector;

        public LineSegmentDecorator( ConnectorUI ui, Point start, Point end, int id)
        {
            Start = start;
            End = end;

            if (start.X > end.X)
            {
                Start = end;
                End = start;
            }
            _id = id;
            _connector = ui;
        }

        public ConnectorUI Connector
        {
            get
            {
                return _connector;
            }
        }

        public bool IsHorizontal
        {
            get
            {
                return Start.Y == End.Y;
            }
        }

        public bool IsVertical
        {
            get
            {
                return Start.X == End.X;
            }
        }

        public Point Start
        {
            get;
            set;
        }

        public Point End
        {
            get;
            set;
        }

        public double MinY
        {
            get
            {
                return Math.Min(Start.Y, End.Y);
            }
        }

        public double MaxY
        {
            get
            {
                return Math.Max(Start.Y, End.Y);
            }
        }


        public static IEnumerable<LineSegmentDecorator> DecorateLineSegments(ConnectorUI ui, int id)
        {
            List<LineSegmentDecorator> lineSegments = new List<LineSegmentDecorator>();
            var geometry = ui.Model.Geometry as GeometryGroup;
            if (geometry != null)
            {
                var path = geometry.Children.First() as PathGeometry;
                if (path != null && path.Figures.Count() > 0)
                {
                    var pathFigureCollection = path.Figures.First();
                    
                    if (pathFigureCollection.Segments.Count() > 0)
                    {
                        var startPoint = pathFigureCollection.StartPoint;
                        foreach (var segment in pathFigureCollection.Segments.OfType<LineSegment>())
                        {
                            var endPoint = segment.Point;
                            lineSegments.Add(
                                new LineSegmentDecorator(ui, startPoint, endPoint, id));

                            startPoint = endPoint;
                        }
                    }
                }
            }
            return lineSegments;
        }

    }
}
