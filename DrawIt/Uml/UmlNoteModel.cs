using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UI.Utilities.Interfaces;
using Sketch.Types;
using Sketch.Models;
using System.Runtime.Serialization;

namespace DrawIt.Uml
{
    [Serializable]
    public class UmlNoteModel:ConnectableBase
    {
        //IList<Bluebottle.Base.Interfaces.ICommandDescriptor> _tools;


        static readonly double TopMargin = 20;
        static readonly double LeftMargin = 2.5;
        static readonly double BottomMargin = 5; // this includes das "eselsohr" 

        string _note;
        FormattedText _formattedNote = null;
        PathFigure[] _notePath = new PathFigure[1];
        

        public UmlNoteModel(Point p )
        {
            Bounds = new Rect(p, new Size(100, 50));
            IsSelected = true;
            AllowSizeChange = true;
            Label = "new note";
            Note = "a note";
            RotationAngle = 0.0;
            FillColor = Colors.White;
            UpdateGeometry();

        }

        protected UmlNoteModel(SerializationInfo info, StreamingContext context)
            :base(info, context) 
        {
            UpdateGeometry();
        }

        public override string LabelPropertyName
        {
            get
            {
                return "Note";
            }
        }

        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                _formattedNote = new FormattedText(Note, System.Globalization.CultureInfo.CurrentCulture,
                    System.Windows.FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Blue,
                    VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip);

                _formattedNote.MaxTextWidth = Math.Max(50, Bounds.Width - 5);
                Rect newBounds = Bounds;
                newBounds.Height = _formattedNote.Extent + BottomMargin * 2 + TopMargin;
                Bounds = newBounds;
                newBounds.Location = new Point(Bounds.Left, Bounds.Top + TopMargin);
                newBounds.Height = _formattedNote.Extent + BottomMargin * 2;
                LabelArea = newBounds;
            }
        }

        public override RectangleGeometry Outline
        {
            get {
                return new RectangleGeometry(Bounds);
            }
        }

        //public override IList<ICommandDescriptor> Tools
        //{
        //    get { return new List<UI.Utilities.Interfaces.ICommandDescriptor>(); }
        //}

        public override void UpdateGeometry()
        {

            var myGeometry = Geometry as GeometryGroup;
            myGeometry.Children.Clear();

            _formattedNote.MaxTextWidth = Math.Max(50, LabelArea.Width - 5);



            //myGeometry.Children.Add(textGeometry);
            _notePath[0] = GetPathFigureFromPoints(new Point[]
                {
                        Bounds.TopLeft,
                        new Point( Bounds.Right - TopMargin, Bounds.Top),
                        new Point( Bounds.Right - TopMargin, Bounds.Top + TopMargin),
                        new Point( Bounds.Right, Bounds.Top + TopMargin),
                        Bounds.BottomRight,
                        Bounds.BottomLeft,
                }
                );

            _notePath[0].IsClosed = true;
            myGeometry.Children.Add(new PathGeometry(_notePath));
            myGeometry.Children.Add(new LineGeometry(new Point(Bounds.Right - TopMargin, Bounds.Top),
                new Point(Bounds.Right, Bounds.Top + TopMargin)));

            myGeometry.Transform = Rotation;
        }

        public override void RenderAdornments(DrawingContext drawingContext)
        {
            var textGeometry = _formattedNote.BuildGeometry(new Point(LabelArea.Left + LeftMargin, LabelArea.Top + BottomMargin));
            drawingContext.DrawGeometry( Brushes.Black, new Pen(Brushes.Black, 0.1),  textGeometry ); 
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Note", Note);
        }

        static PathFigure GetPathFigureFromPoints( IEnumerable<Point> linePoints )
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
            return pf;
        }

        protected override void RestoreData(SerializationInfo info, StreamingContext context)
        {
            base.RestoreData(info, context);
            Note = info.GetString("Note");
        }
    }
}
