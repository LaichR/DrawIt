using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Sketch.Models
{
    /// <summary>
    /// Model of a label that belongs to a connector. 
    /// A label can be attached to the  starting point or the end point of a connector
    /// </summary>
    public class ConnectorLabelModel : ConnectableBase
    {
        ConnectorModel _connector;
        bool _isStartpointLabel;
        Point _labelPosition;
        RectangleGeometry _outline;
        FormattedText _text;
        Geometry _geometry;
        List<UI.Utilities.Interfaces.ICommandDescriptor> _tools = new List<UI.Utilities.Interfaces.ICommandDescriptor>();

        public ConnectorLabelModel(ConnectorModel connector, bool isStartPointLabel, Point labelPosition)
        {
            _connector = connector;
            _connector.PropertyChanged += _connector_PropertyChanged;
            _labelPosition = labelPosition;
            _isStartpointLabel = isStartPointLabel;
            AllowSizeChange = false;
            _text = new FormattedText(_connector.Name, System.Globalization.CultureInfo.CurrentCulture,
              System.Windows.FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Blue);
            _geometry = _text.BuildGeometry(ComputeTextPosition(_labelPosition));

            Bounds = ComputeBounds(_labelPosition);
            LabelArea = Bounds;

            _outline = new RectangleGeometry(Bounds);
            FillColor = Colors.Snow;
        }

        void _connector_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if( e.PropertyName == "Name" )
            {
                _text = new FormattedText(_connector.Name, System.Globalization.CultureInfo.CurrentCulture,
                    System.Windows.FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Blue);
                OnPropertyChanged("Geometry");
                OnPropertyChanged("ConnectorLabel");
            }
        }

        

        public override string EditableLabelName
        {
            get { return "ConnectorLabel"; }
        }


        public string ConnectorLabel
        {
            get
            {
                return _connector.Name;
            }
            set
            {
                _connector.Name = value;
            }
        }

        //public Brush Fill
        //{
        //    get
        //    {
        //        return Brushes.Snow;
        //    }
        //}

        public double StrokeThickness
        {
            get
            {
                return 0.1;
            }
        }

        public DoubleCollection StrokeDashArray
        {
            get
            {
                return new DoubleCollection(new double[]{0.5, 0.2});
            }
        }

        public override Rect LabelArea
        {
            get
            {
                return _connector.LabelArea;
            }
            set
            {
                _connector.LabelArea = value;
                OnPropertyChanged("LabelArea");
            }
        }

        public override void RenderAdornments(DrawingContext drawingContext)
        {
            drawingContext.DrawText(_text, ComputeTextPosition(_labelPosition));
        }

        public override System.Windows.Media.RectangleGeometry Outline
        {
            get { return _outline; }
        }

        public override void Move(Transform translation)
        {
            _labelPosition = translation.Transform(_labelPosition);
            var newBounds = ComputeBounds(_labelPosition);
            _outline = new RectangleGeometry(newBounds);
            // this assignement trigger the update process of the UI
            LabelArea = newBounds;
            Bounds = newBounds;
        }

        //public override IList<UI.Utilities.Interfaces.ICommandDescriptor> Tools
        //{
        //    get { return _tools; }
        //}

        public override System.Windows.Media.Geometry Geometry
        {
            get
            {
                var geometry = new GeometryGroup();
                //var textGeometry = _text.BuildGeometry(ComputeTextPosition(_labelPosition));
                var endPoint = ConnectorUtilities.Intersect(_outline.Bounds,
                                                ConnectorUtilities.ComputeCenter(_outline.Bounds), ConnectorReferencePoint);
                var line = new LineGeometry(endPoint, ConnectorReferencePoint);
                geometry.Children.Add(
                    Outline
                    );
                geometry.Children.Add(line);
                return geometry;
            }
        }

        internal void InvalidateGeometry()
        {
            OnPropertyChanged("Geometry");
        }

        Point ConnectorReferencePoint
        {
            get
            {
                return _isStartpointLabel ? _connector.ConnectorStart : _connector.ConnectorEnd;
            }
        }

        Point ComputeTextPosition( Point pos)
        {
            return new Point(pos.X + 10, pos.Y + 8);
        }

        Rect ComputeBounds( Point origin)
        {
            
            return new Rect(origin, new Size(_text.Width + 20, _text.Height + 16));
        }
    }
}
