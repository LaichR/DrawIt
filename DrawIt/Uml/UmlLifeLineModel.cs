using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Bluebottle.Base.Interfaces;
using Sketch.Types;
using Sketch.Models;
using System.Runtime.Serialization;

namespace Drawing.Uml
{
    [Serializable]
    public class UmlLifeLineModel : ConnectableBase
    {
        IList<Bluebottle.Base.Interfaces.ICommandDescriptor> _tools = new List<Bluebottle.Base.Interfaces.ICommandDescriptor>();
        OutlineToolFactory _toolFactory;
        double _height;
        double _activityStart = 0.05;
        public UmlLifeLineModel(Size parentSize, Point p, OutlineToolFactory toolFactory)
        {
            var pos = p;
            pos.Y = 10;
            IsSelected = true;
            AllowSizeChange = true;
            Name = "a Object";
            RotationAngle = 0.0;
            FillColor = Colors.White;
            _toolFactory = toolFactory;

            FormattedText t = new FormattedText(Name, System.Globalization.CultureInfo.CurrentCulture,
                     System.Windows.FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Blue);
            _height = parentSize.Height;
            
            LabelArea = new Rect(pos, new Size(t.Width, t.Height));
            pos.X = (LabelArea.Left + LabelArea.Right) / 2;
            pos.Y = LabelArea.Bottom;
            Bounds = new Rect( pos, new Size(1, parentSize.Height));
            AllowSizeChange = false;
            _tools = _toolFactory.GetTools();
        }

        internal double GetNextInteractionPosition()
        {
            var ret = _activityStart;
            _activityStart += 0.05;
            return ret;
        }

        public override RectangleGeometry Outline
        {
            get
            {
                return new RectangleGeometry(Bounds);
            }
        }

        public override IList<ICommandDescriptor> Tools
        {
            get
            {
                return _tools;
            }
        }


        public override Geometry Geometry
        {
            get
            {
                var myGeometry = new GeometryGroup();

                FormattedText t = new FormattedText(Name, System.Globalization.CultureInfo.CurrentCulture,
                    System.Windows.FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Blue);
                //var textGeometry = t.BuildGeometry();
                var labelRect = new RectangleGeometry(
                    new Rect(new Point(LabelArea.Left + 7, LabelArea.Top + 2),
                             new Point(LabelArea.Right + 7, LabelArea.Bottom + 2)));

                myGeometry.Children.Add(labelRect);

                List<Point> points = new List<Point>
                {
                    new Point( (LabelArea.Left + LabelArea.Right)/2-1, LabelArea.Bottom  ),
                    new Point( (LabelArea.Left + LabelArea.Right)/2 + 1, _height)
                };

                LineGeometry line = new LineGeometry(points[0], points[1]);

                myGeometry.Children.Add(line);
                return myGeometry;
            }
        }
    }
}
