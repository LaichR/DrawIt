using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Sketch.Types;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using Sketch.Models;

namespace Sketch.Controls
{
    internal class IntersectionFinder: Adorner
    {

        internal class Intersection
        {
            IntersectionFinder _parent;
            Rect _toRestoreRect;
            Point _oldIntersectionPoint;
            Pen _toRestorePen;
            Brush _toRestoreBrush;

            public Intersection( IntersectionFinder parent)
            {
                _parent = parent;
            }

            
            public Point IntersectionPoint
            {
                get;
                set;
            }

            public ConnectorUI Intersecting1 
            {
                get;
                set;
            }

            public ConnectorUI Intersecting2
            {
                get;
                set;
            }

            internal void Draw(DrawingContext drawingContext, bool showIntersection)
            {
                var rect = new Rect(new Point(IntersectionPoint.X - INTERSECTION_RECTANGLE + 3,
                   IntersectionPoint.Y - INTERSECTION_RECTANGLE+1),
                   new Size(2 * INTERSECTION_RECTANGLE, 2 * INTERSECTION_RECTANGLE-2));

                _toRestoreBrush = Intersecting2.Fill;
                _toRestorePen = new Pen(Intersecting2.LineBrush, Intersecting2.LineWidth);

                if ( showIntersection)
                {
                       
                    drawingContext.DrawRectangle(Brushes.White, null, rect);
                    DrawIntersection(drawingContext, new Pen(Intersecting1.LineBrush, Intersecting1.LineWidth));
                    //drawingContext.DrawLine(new Pen(Intersecting1.LineBrush, Intersecting1.LineWidth),
                    //    new Point(IntersectionPoint.X, IntersectionPoint.Y - INTERSECTION_RECTANGLE),
                    //    new Point(IntersectionPoint.X, IntersectionPoint.Y + INTERSECTION_RECTANGLE));

                    _oldIntersectionPoint = IntersectionPoint;
                    _toRestoreRect = rect;
                    //_toRestoreBrush = Intersecting2.Fill;
                    //_toRestorePen = new Pen(Intersecting2.LineBrush, Intersecting2.LineWidth);
                }
                else
                {
                    // both lines were moved
                    // drawingContext.DrawRectangle(Brushes.White, null, rect);

                    drawingContext.PushClip(new RectangleGeometry(_toRestoreRect));
                    // vertical line not was moved
                    var pen = _toRestorePen;
                    if (Intersecting2.Model.Geometry.StrokeContains(_toRestorePen, _oldIntersectionPoint))
                    {
                        drawingContext.DrawGeometry(Intersecting1.Fill, pen,
                            Intersecting2.Model.Geometry);
                    }
                    // horizontal line was not moved
                    pen = _toRestorePen;
                    if (Intersecting2.Model.Geometry.StrokeContains(pen, _oldIntersectionPoint))
                    {
                        
                        drawingContext.DrawGeometry(Intersecting2.Fill, pen,
                            Intersecting2.Model.Geometry);

                    }
                    drawingContext.Pop();

                }

            }

            void DrawIntersection(DrawingContext context, Pen pen)
            {
                var rect = new Rect(new Point(IntersectionPoint.X,
                    IntersectionPoint.Y - INTERSECTION_RECTANGLE),
                    new Size(INTERSECTION_RECTANGLE*2, 2 * INTERSECTION_RECTANGLE));
                context.PushClip(new RectangleGeometry(rect));
                context.DrawEllipse(null, pen, IntersectionPoint, INTERSECTION_RECTANGLE / 2 + 1, INTERSECTION_RECTANGLE);
                context.Pop();
                //    new Point(_oldIntersectionPoint.X - INTERSECTION_RECTANGLE, _oldIntersectionPoint.Y ),
                //    new Point(_oldIntersectionPoint.X + INTERSECTION_RECTANGLE, _oldIntersectionPoint.Y ));
            }
        }

        bool _isRendering = false;
        object synchRoot = new object();
        Intersection[] _intersections = new Intersection[0];

        private const int INTERSECTION_RECTANGLE = 5;
        
        LinkedAvlTree<ScanLine> _horizontalScan = null;// _horizontalScan new LinkedAvlTree<ScanLine>();
        LinkedAvlTree<ScanLine> _verticalScan = null;// new LinkedAvlTree<ScanLine>();

        int lineSegments = 0;
        SketchPad _sketchPad;


        public IntersectionFinder(UIElement sketchPad) : base(sketchPad)
        {
            _sketchPad = AdornedElement as SketchPad;
            RuntimeCheck.Assert.True(_sketchPad != null, "Invalid sketchpad");
            IsHitTestVisible = false;
            Visibility = Visibility.Visible;

        }


        protected override Size MeasureOverride(Size constraint)
        {
            return new Size();
        }


        void AddLineSegments(ConnectorUI ui)
       {
            foreach (LineSegmentDecorator l in LineSegmentDecorator.DecorateLineSegments(ui, lineSegments++))
            {
                if (l.IsHorizontal && l.IsVertical) // this is just a point!
                {
                    continue;
                }
                _horizontalScan.Add(new ScanLine(l, l.Start.X));
                if (l.IsHorizontal)
                {
                    System.Diagnostics.Debug.Assert(l.Start.X < l.End.X);
                    _horizontalScan.Add(new ScanLine(l, l.End.X));
                }
            }

        }

        internal Intersection[] ComputeIntersections()
        {
                List<Intersection> intersections = new List<Intersection>();
                foreach (ScanLine l in _horizontalScan.ToArray())
                {
                    double scanPos = l.ScanPos;
                    // all horizontal lines if it's a starting point
                    foreach (LineSegmentDecorator ls in l.HorizontalLines.Where((x) => (x.Start.X == scanPos)))
                    {
                        _verticalScan.Add(new ScanLine(ls, ls.Start.Y));
                    }

                    // check for interesections
                    foreach (LineSegmentDecorator ls in l.VertiacalLines)
                    {
                        LinkedAvlTreeNode<ScanLine> verticalScan = _verticalScan.LowerBound(new ScanLine(ls.MinY));
                        while (verticalScan != null)
                        {
                            double y = verticalScan.Data.ScanPos;
                            if (y > ls.MinY && y < ls.MaxY)
                            {
                                var intersection = new Intersection(this)
                                {
                                    IntersectionPoint = new Point(scanPos, y),
                                    Intersecting1 = ls.Connector,
                                    Intersecting2 = verticalScan.Data.Connector,
                                };

                                intersections.Add(intersection);

                            }
                            
                            else if (y > ls.MaxY) break;
                            verticalScan = verticalScan.Next;
                        }
                    }

                    // remove all finished lines from the vertical scan line collection
                    foreach (LineSegmentDecorator ls in l.HorizontalLines.Where((x) => (x.End.X == scanPos)))
                    {
                        LinkedAvlTreeNode<ScanLine> vs = _verticalScan.LowerBound(new ScanLine(ls.Start.Y));
                        if (vs == null || vs.Data.ScanPos != ls.Start.Y)
                        {
                            continue;
                        }


                        System.Diagnostics.Debug.Assert(vs.Data.ScanPos == ls.Start.Y);
                        vs.Data.Remove(ls);
                        if (vs.Data.Count == 0)
                        {
                            _verticalScan.Delete(vs.Data);
                        }
                    }
                }
                return intersections.ToArray();
            
        }


        protected override void OnRender(DrawingContext drawingContext)
        {

            if (_sketchPad.CurrentOperationHandler != null &&
                !(_sketchPad.CurrentOperationHandler is SelectUisOperation )) return;

            foreach (var @i in _intersections)
            {
                i.Draw(drawingContext, false);
            }

            //List<ConnectorUI> toRefresh = new List<ConnectorUI>(
            //    _intersections.Select<Intersection, ConnectorUI>(
            //        (x) =>
            //        {
            //            x.Draw(drawingContext, false);
            //            var ret = x.Intersecting1;
            //            if (ret == _connector)
            //            {
            //                ret = x.Intersecting2;
            //            };
            //            return ret;
            //        }));


            _horizontalScan = new LinkedAvlTree<ScanLine>();
            _verticalScan = new LinkedAvlTree<ScanLine>();

            //_isRendering = true; // only one IntersectionFinder needs to render
            foreach (var connector in _sketchPad.Children.OfType<ConnectorUI>())
            {
                AddLineSegments(connector);
            }

            _intersections = ComputeIntersections();

            foreach (var intersection in _intersections)
            {
                intersection.Draw(drawingContext, true);
            }
        }
    }
}
