using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UI.Utilities.Interfaces;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Prism.Commands;
using UI.Utilities;
using Sketch.Interface;
using System.Drawing.Text;
using System.CodeDom;
using Sketch.Types;
using System.ComponentModel;

namespace Sketch.Models
{

    public abstract class ConnectableBase: ModelBase, IBoundedItemModel
    {

        public static readonly double DefaultWidth = 100;
        public static readonly double DefaultHeight = 100;
        public static readonly double MinimalTextMarginX = 5;
        public static readonly double MinimalTextMarginY = 2;
        public const double DefaultFontSize = 12;
        public static readonly Color DefaultColor = Colors.Snow;
        public static readonly Brush DefaultStroke = Brushes.Black;
        public static readonly Typeface DefaultFont = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.UltraLight, FontStretches.UltraExpanded);

        Rect _bounds;
        
        bool _allowSizeChange;
        double _rotationAngle = 0.0;
        RotateTransform _rotateTransform = null;
        SerializableColor _fillColor = new SerializableColor();
        
        
        double _fontSize = DefaultFontSize;
        Brush _stroke = DefaultStroke;
        double _strokeThickness = 1;

        DelegateCommand _cmdSelectColor;
        GeometryGroup _geometry = new GeometryGroup();

        public ConnectableBase(Point pos, Size size, 
            string label,
            Color color) 
        {
            IsSelected = true;
            Label = label;
            FillColor = color;
            LabelArea = ComputeLabelArea(DisplayedLabel());
            _bounds = ComputeBounds(pos, size, LabelArea);
            
            Initialize();
        }

        public Brush Stroke
        {
            get => _stroke;
            set
            {
                SetProperty<Brush>(ref _stroke, value);
            }
        }

        public double StrokeThickness
        {
            get => _strokeThickness;
            set
            {
                SetProperty<double>(ref _strokeThickness, value);
            }
        }

        public virtual IList<ICommandDescriptor> AllowableConnectors
        {
            get => SketchItemFactory.ActiveFactory.GetAllowableConnctors(this.GetType());
        }

        protected ConnectableBase(SerializationInfo info, StreamingContext context):base(info, context)
        {}

        public override void UpdateGeometry()
        {
            _geometry.Children.Clear();
            _geometry.Children.Add(new RectangleGeometry(Bounds));
        }

        public virtual Point GetPreferredToolsLocation( Point curMouse, out ConnectorDocking docking )
        {
            docking = ConnectorDocking.Undefined;
            var leftThreshold = Bounds.X + Bounds.Width / 5;
            var rightThreshold = Bounds.X + Bounds.Width / 5 * 4;
            var topThreshold = Bounds.Y + Bounds.Height / 5;
            var bottomThreshold = Bounds.Y + Bounds.Height / 5 * 4;
            
            var pos = Bounds.TopRight; // default position
            docking = ConnectorDocking.Right;
            if( curMouse.X < leftThreshold)
            {
                pos = Bounds.TopLeft;
                docking = ConnectorDocking.Left;
            }
            if( curMouse.Y <= topThreshold)
            {
                pos = Bounds.TopLeft;
                docking = ConnectorDocking.Top;
            }
            else if( curMouse.Y >= bottomThreshold)
            {
                pos = Bounds.BottomLeft;
                docking = ConnectorDocking.Bottom;
            }

            return pos;
        }

        public virtual ConnectorDocking AllowableDockings(bool incoming)
        {
            return ConnectorDocking.Bottom | ConnectorDocking.Left | 
                ConnectorDocking.Right | ConnectorDocking.Top;
        }

        public override Geometry Geometry => _geometry;

        public override void RenderAdornments(DrawingContext drawingContext)
        {
            // draw the lable
            if (LabelArea != Rect.Empty)
            {
                RenderLabel(DisplayedLabel(), drawingContext);
            }
        }

        [Browsable(true)]
        public Color FillColor
        {
            get
            {
                return _fillColor.Color;
            }
            set
            {
                _fillColor.Color = value;
                RaisePropertyChanged("Fill");
                RaisePropertyChanged("FillColor");
            }
        }

        public virtual Brush Fill
        {
            get
            {
                return _fillColor.Brush;
            }
            
        }

        public RotateTransform Rotation
        {
            get
            {
                if (_rotateTransform == null )
                {
                    _rotateTransform =  new RotateTransform(_rotationAngle, (_bounds.Left + _bounds.Right) / 2, (_bounds.Top + _bounds.Bottom) / 2);
                }
                return _rotateTransform;
            }
        }

        [Browsable(true)]
        public virtual Rect Bounds
        {
            get
            {
                return _bounds;
            }
            set
            {
                SetProperty<Rect>(ref _bounds, value);
            }
        }

        public bool AllowSizeChange
        {
            get { return _allowSizeChange; }
            set { SetProperty<bool>(ref _allowSizeChange, value); }
        }

        public virtual RectangleGeometry Outline
        {
            get => new RectangleGeometry(Bounds);
        }

        public virtual Point GetConnectorPoint(ConnectorDocking docking, double relativePosition)
        {
            return ConnectorUtilities.ComputePoint(Bounds, docking, relativePosition);
        }

        public virtual Point GetPreferredConnectorStart(Point hint, out double relativePos, out ConnectorDocking docking)
        {
            return GetConnectorPositionInfo(hint, out relativePos, out docking);
        }

        public virtual Point GetPreferredConnectorEnd(Point hint, out double relativePos, out ConnectorDocking docking)
        {
            return GetConnectorPositionInfo(hint, out relativePos, out docking);
        }

        protected virtual Point GetConnectorPositionInfo(Point hint, out double relativePos, out ConnectorDocking docking)
        {
            var center = ConnectorUtilities.ComputeCenter(Bounds);
            if (Bounds.Contains(hint))
            {
                if (center.X != hint.X)
                {
                    hint.X = hint.X - 000;
                    hint.Y = hint.Y - 500 * ConnectorUtilities.Slope(hint, center);
                }
                else
                {
                    hint.Y = 0;
                }
            }
            Point p = ConnectorUtilities.Intersect(Bounds, hint, center);
            double pos = 0.0;
            docking = ConnectorUtilities.ComputeDocking(Bounds, p, ref pos);
            relativePos = pos;
            return p;
        }

        protected virtual FormattedText ComputeFormattedText(
                        string label,  
                        Typeface typeface, double size,
                        FlowDirection flowDirection = System.Windows.FlowDirection.LeftToRight
            )
        {
            size = Math.Max(8, size);
            return new FormattedText(label, System.Globalization.CultureInfo.CurrentCulture,
                    flowDirection, typeface, size, Stroke,
                    VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip);
        }

        protected virtual Size ComputeFormattedTextSize(string label,
                        Typeface typeface, double size, 
                        double marginX, double marginY)
        {
            var formattedText = ComputeFormattedText(label, typeface, size);
            var textSize = new Size(formattedText.WidthIncludingTrailingWhitespace + marginX,
                formattedText.Height + formattedText.LineHeight + marginY);
            return textSize;
        }


        /// <summary>
        /// provide an interface to populate a toolbar from close to the view!
        /// </summary>
        //public abstract IList<ICommandDescriptor> Tools
        //{
        //    get;
        //}


        protected virtual void RenderLabel(string displayed, DrawingContext drawingContext)
        {
            // draw the lable
            FormattedText t = new FormattedText(displayed, System.Globalization.CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight, DefaultFont, _fontSize,
                System.Windows.Media.Brushes.Black,
                VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip);
            var position = new System.Windows.Point(LabelArea.Left + MinimalTextMarginX, 
                LabelArea.Top + MinimalTextMarginY);
            drawingContext.DrawText(t, position);
        }

        protected virtual string DisplayedLabel()
        {
            return Label;
        }

        public double RotationAngle
        {
            get { return _rotationAngle; }
            protected set
            {
                _rotationAngle = value;
                _rotateTransform = null;
            }
        }

        public override void Move(Transform translation)
        {
            if (translation == null) return;


            _rotateTransform = null;
            
            var oldBounds = Bounds;
            var newBounds = oldBounds;

            newBounds.Transform(translation.Value);
            if (LabelArea != Rect.Empty)
            {
                LabelArea = ComputeLabelArea(DisplayedLabel()); // the label area is relative to the 
            }
            
            Bounds = newBounds;
            UpdateGeometry();
            NotifyShapeChanged( ref oldBounds, ref newBounds);
        }

        public event EventHandler<OutlineChangedEventArgs> ShapeChanged;



        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("AllowSizeChange", AllowSizeChange);
            info.AddValue("Bounds", Bounds);
            info.AddValue("Fill", _fillColor.ScRgb );
        }

        public ConnectableBase Clone()
        {
            using (var inputStream = new System.IO.MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(inputStream, this);
                var data = inputStream.ToArray();
                using (var outputStream = new System.IO.MemoryStream(data))
                {
                    var newObj = formatter.Deserialize(outputStream) as ConnectableBase;
                    return newObj;
                }
            }
        }

        #region private helpers

        protected virtual Rect ComputeLabelArea(string label)
        {
            if (!string.IsNullOrEmpty(Label))
            {
                return new Rect(
                    new Point(5, 5),
                    ComputeFormattedTextSize(label, DefaultFont, DefaultFontSize, 10, 10));
            }
            return Rect.Empty;
        }

        protected virtual Rect ComputeBounds(Point pos, Size size, Rect labelArea )
        {

            return new Rect(pos,
                new Size(Math.Max(labelArea.Width + 5, size.Width),
                         Math.Max(labelArea.Height + 5, size.Height)));
            
        }

        protected virtual void NotifyShapeChanged(ref Rect old, ref Rect @new)
        {
            if (ShapeChanged != null)
            {
                ShapeChanged(this, new OutlineChangedEventArgs(old, @new));
            }
        }
        void SelectColor()
        {
            var dlg = new Controls.ColorPicker.ColorPickerDialog();
            dlg.StartingColor = _fillColor.Color;
            bool? result = dlg.ShowDialog();

            if( result ?? true)
            {
                this.FillColor = dlg.SelectedColor;
            }
        }

        

        protected override void RestoreFieldData(SerializationInfo info, StreamingContext context)
        {
            base.RestoreFieldData(info, context);
            _fillColor = new SerializableColor();
            _bounds = (Rect)info.GetValue("Bounds", typeof(Rect));
            _allowSizeChange = info.GetBoolean("AllowSizeChange");
            try
            {
                _fillColor.ScRgb = (float[])info.GetValue("Fill", typeof(float[]));
            }
            catch { }
            _fillColor = new SerializableColor();
            _bounds = (Rect)info.GetValue("Bounds", typeof(Rect));
            _allowSizeChange = info.GetBoolean("AllowSizeChange");
            try
            {
                _fillColor.ScRgb = (float[])info.GetValue("Fill", typeof(float[]));
            }
            catch { }

            //Initialize();
        }

        protected override void Initialize()
        {
            
            _cmdSelectColor = new DelegateCommand(SelectColor);
            Commands = new List<ICommandDescriptor>();
            Commands.Add(new UI.Utilities.Behaviors.CommandDescriptor
            {
                Name = "Select Color",
                Command = _cmdSelectColor
            });
        }



        static Color GetStopColor(Color color )
        {
            Color stopColor = new Color
            {
                A = (byte)(color.A-16),
                R = (byte)(Colors.Wheat.R - 32),
                G = (byte)(Colors.Wheat.G - 32),
                B = (byte)(Colors.Wheat.B - 32),
            };
            return stopColor;
        }

        #endregion
    }
}
