using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sketch.Models;
using System.Windows;
using System.Windows.Media;
using UI.Utilities.Interfaces;
using Sketch.Interface;
using System.Runtime.Serialization;


namespace DrawIt.Uml
{
    [Serializable]
    [AllowableConnector(typeof(UmlDependencyModel))]
    [AllowableConnector(typeof(UmlAssociationModel))]
    public class UmlPackageModel: ContainerModel
    {
        const int DefaultWidth = 150;
        const int DefaultHeight = 95;
        public UmlPackageModel( Point p )
            : base(p, new Size( DefaultWidth, DefaultHeight)) 
        {
            var location = new Point(p.X + 5, p.Y);
            LabelArea = new Rect(location, new Size(140, 20));
            IsSelected = true;
            AllowSizeChange = true;
            Label = "new package";
            UpdateGeometry();
        }

        protected UmlPackageModel(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }

        public override void UpdateGeometry()
        {
            
                var myGeometry = Geometry as GeometryGroup;
                myGeometry.Children.Clear();

                FormattedText t = new FormattedText(Label, System.Globalization.CultureInfo.CurrentCulture,
                    System.Windows.FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Blue,
                    VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip);

                var textGeometry = t.BuildGeometry(new Point(LabelArea.Left + 7, LabelArea.Top + 2));

                //myGeometry.Children.Add(textGeometry);

                List<Point> points = new List<Point>
                {
                    new Point( LabelArea.Left -2, LabelArea.Top + LabelArea.Height  ),
                    new Point( LabelArea.Left, LabelArea.Top + LabelArea.Height - 2 ),
                    new Point( LabelArea.Left + 2, LabelArea.Top + 2 ),
                    new Point( LabelArea.Left+4, LabelArea.Top ),
                    new Point( textGeometry.Bounds.Right + 2, LabelArea.Top ),
                    new Point( textGeometry.Bounds.Right + 4, LabelArea.Top + 1 ),
                    new Point( textGeometry.Bounds.Right + 7, LabelArea.Top + 5 ),
                    new Point( textGeometry.Bounds.Right + 10, LabelArea.Top + LabelArea.Height - 2 ),
                    new Point( textGeometry.Bounds.Right + 14, LabelArea.Bottom )
                };

                
                PathGeometry labelPath = new PathGeometry(GetLabelBorder(points));
                //var screenLocation = this.PointToScreen(location);

                myGeometry.Children.Add(labelPath);
                var body = new Rect(Bounds.Left, Bounds.Top + LabelArea.Height, Bounds.Width, Bounds.Height - LabelArea.Height);
                myGeometry.Children.Add(new RectangleGeometry(body));
        }

        public override System.Windows.Media.RectangleGeometry Outline
        {
            get
            {
                return new System.Windows.Media.RectangleGeometry(Bounds);
            }
        }

        public static IEnumerable<PathFigure> GetLabelBorder( IEnumerable<Point> linePoints )
        {
            var pf = new PathFigure();
            System.Windows.Media.PathSegmentCollection ls = new System.Windows.Media.PathSegmentCollection();
            var start = linePoints.First();
            foreach (var p in linePoints.Skip(1))
            {
                ls.Add(new System.Windows.Media.LineSegment(p, true));
            }
            pf.StartPoint = start;
            pf.Segments = ls;
            return new List<PathFigure>{pf};
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
