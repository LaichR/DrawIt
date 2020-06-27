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
    /// 
    public class ConnectorLabelModel : ConnectableBase
    {
        bool _geometryUpdating = false;
        ConnectorModel _connector;
        bool _isStartpointLabel;
        System.Windows.Point _labelPosition;
        RectangleGeometry _outline;
        LineGeometry _linkToConnector;
        FormattedText _formattedText;
        
        List<UI.Utilities.Interfaces.ICommandDescriptor> _tools = new List<UI.Utilities.Interfaces.ICommandDescriptor>();

        public ConnectorLabelModel(ConnectorModel connector, bool isStartPointLabel, Point labelPosition)
        {
            _connector = connector;
            _connector.PropertyChanged += _connector_PropertyChanged;
            _labelPosition = labelPosition;
            _isStartpointLabel = isStartPointLabel;
            AllowSizeChange = false;
            FillColor = Colors.Snow;
            StrokeThickness = 0.2;
            UpdateGeometry();
        }

        public override bool IsSerializable
        {
            get => false;
        }

        void _connector_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if( e.PropertyName == "Name" )
            {
                UpdateGeometry();
            }
        }

        public override string LabelPropertyName => "ConnectorLabel";

        public string ConnectorLabel
        {
            get
            {
                return _connector.Label;
            }
            set
            {
                _connector.Label = value;
                UpdateGeometry();
            }
        }

        //public Brush Fill
        //{
        //    get
        //    {
        //        return Brushes.Snow;
        //    }
        //}

       

        public DoubleCollection StrokeDashArray
        {
            get
            {
                return new DoubleCollection(new double[]{3, 3});
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
                Bounds = value;
            }
        }

        public override void RenderAdornments(DrawingContext drawingContext)
        {
            drawingContext.DrawText(_formattedText, ComputeTextPosition(_labelPosition));
            drawingContext.Pop();
            drawingContext.DrawGeometry(Brushes.LightGray,
                new Pen(Brushes.DarkGray, 0.5), _linkToConnector);
            drawingContext.PushClip(_outline);
        }

        public override System.Windows.Media.RectangleGeometry Outline
        {
            get { return new RectangleGeometry(Bounds); }
        }

        public override void Move(Transform translation)
        {
            _labelPosition = translation.Transform(_labelPosition);
            var newBounds = ComputeBounds(_labelPosition);
            // this assignement trigger the update process of the UI
            LabelArea = newBounds;
            UpdateGeometry();
        }

        //public override IList<UI.Utilities.Interfaces.ICommandDescriptor> Tools
        //{
        //    get { return _tools; }
        //}

        public override void UpdateGeometry()
        {
            if (!_geometryUpdating && _connector !=null)
            {
                _geometryUpdating = true; // since 
                _formattedText = new FormattedText(_connector.Label, System.Globalization.CultureInfo.CurrentCulture,
                    System.Windows.FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Blue,
                    VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip);

                LabelArea = ComputeBounds(_labelPosition);
                
                _outline = new RectangleGeometry(LabelArea);
                var g = base.Geometry as GeometryGroup;
                g.Children.Clear();
                var endPoint = ConnectorUtilities.Intersect(LabelArea,
                               ConnectorUtilities.ComputeCenter(LabelArea), ConnectorReferencePoint);
                _linkToConnector = new LineGeometry(endPoint, ConnectorReferencePoint);
                
                g.Children.Add(_outline);
                
                _geometryUpdating = false;
            }
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
            return new Rect(origin, new Size(_formattedText.Width + 20, _formattedText.Height + 16));
        }
    }
}
