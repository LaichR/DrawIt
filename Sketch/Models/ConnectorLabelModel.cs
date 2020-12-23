using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Sketch.Interface;

namespace Sketch.Models
{
    /// <summary>
    /// Model of a label that belongs to a connector. 
    /// A ConnectorLabelModel can be attached to 
    /// - the  starting point 
    /// - the end point of a connector
    /// A connector may have two labels at the same time
    /// The Label of the connector always refers to the Label of the Starting Point.
    /// 
    /// </summary>
    /// 
    [DoNotConsiderForConnectorRouting]
    public class ConnectorLabelModel : ConnectableBase
    {
        bool _geometryUpdating = false;
        readonly ConnectorModel _connector;
        readonly bool _isStartpointLabel;
        readonly bool _showLabelConnection;
        
        LineGeometry _linkToConnector;

        public ConnectorLabelModel(ConnectorModel connector, ISketchItemContainer container, bool isStartPointLabel, bool showLabelConnection, Point labelPosition)
            :base(labelPosition, container,
                 ComputeSizeOfBounds(connector.GetLabel(isStartPointLabel)), 
                 connector.GetLabel(isStartPointLabel), Colors.Snow )
        {
            _connector = connector;
            _connector.PropertyChanged += Connector_PropertyChanged;
            _isStartpointLabel = isStartPointLabel;
            _showLabelConnection = showLabelConnection;
            CanChangeSize = false;
            StrokeThickness = 0.2;
            FillColor = Color.FromArgb(0x31, 0xFF, 0xFA, 0xFA); 
            UpdateGeometry();
        }

        public override bool IsSerializable
        {
            get => false;
        }

        void Connector_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if( e.PropertyName == nameof(Label) || e.PropertyName == nameof(Geometry) )
            {
                AdjustBounds();
                RaisePropertyChanged(nameof(ConnectorLabel));   
            }
        }

        public override string LabelPropertyName => "ConnectorLabel";

        [Browsable(true)]
        public string ConnectorLabel
        {
            get
            {
                return _connector.GetLabel(_isStartpointLabel);
            }
            set
            {
                _connector.SetLabel(_isStartpointLabel, value );
                AdjustBounds();
                RaisePropertyChanged();
            }
        }

        [Browsable(false)]
        public override string Label { get => base.Label; set => base.Label = value; }

        void AdjustBounds()
        {
            if (Bounds.Left != 0) // the bounds where not yet initialized
            {
                LabelArea = ComputeLabelArea(DisplayedLabel());
                var w = Math.Max(25, LabelArea.Width + 20);
                var h = Bounds.Height;
                Bounds = ComputeBounds(Bounds.TopLeft, new Size(w, h), LabelArea);
            }
        }

        protected override string DisplayedLabel()
        {
            return _connector == null ? Label: ConnectorLabel;
        }


        public DoubleCollection StrokeDashArray
        {
            get
            {
                return new DoubleCollection(new double[]{3, 3});
            }
        }



        public override void RenderAdornments(DrawingContext drawingContext)
        {
            //drawingContext.DrawText(_formattedText, ComputeTextPosition(new Point(0,0)));
            if (_showLabelConnection)
            {
                drawingContext.Pop();

                drawingContext.DrawGeometry(Brushes.LightGray,
                    new Pen(Brushes.DarkGray, 0.5), _linkToConnector);
                drawingContext.PushClip(Outline);
            }
        }

        public override void Move(Transform translation)
        {
            base.Move(translation);
            
            UpdateGeometry();
        }

        public override Rect Bounds
        {
            get => base.Bounds;

            set
            {
                base.Bounds = value;
                _connector.SetLabelArea(_isStartpointLabel, Bounds);
            }
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

                var r = new Rect(0, 0, Bounds.Width, Bounds.Height);

                
                var g = base.Geometry as GeometryGroup;
                g.Children.Clear();
                if (!Bounds.Contains(ConnectorReferencePoint))
                {
                    var endPoint = ConnectorUtilities.Intersect(Bounds,
                                   ConnectorUtilities.ComputeCenter(Bounds), ConnectorReferencePoint);

                    _linkToConnector = new LineGeometry(endPoint, ConnectorReferencePoint);
                }
                else
                {
                    _linkToConnector = new LineGeometry(ConnectorReferencePoint, ConnectorReferencePoint);
                    
                }
                _linkToConnector.Transform = new TranslateTransform()
                {
                    X = -Bounds.Left,
                    Y = -Bounds.Top
                };
                g.Children.Add(new RectangleGeometry(r));
                
                _geometryUpdating = false;
                RaisePropertyChanged(nameof(Bounds));
            }
        }


        Point ConnectorReferencePoint
        {
            get
            {
                return _isStartpointLabel ? _connector.ConnectorStart : _connector.ConnectorEnd;
            }
        }

        //Point ComputeTextPosition( Point pos)
        //{
        //    return new Point(pos.X + 10, pos.Y + 8);
        //}

        static FormattedText ComputeFormattedText( string label )
        {
            return new FormattedText(label, System.Globalization.CultureInfo.CurrentCulture,
                    System.Windows.FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Blue,
                    VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip);
        }

        static Size ComputeSizeOfBounds(  string label)
        {
            var formattedText = ComputeFormattedText(label);
            return new Size(formattedText.Width + 20, formattedText.Height + 16);
        }
    }
}
