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

namespace Sketch.Models
{

    public abstract class ConnectableBase: ModelBase, IBoundedItemModel
    {

        public static readonly double DefaultWidth = 100;
        public static readonly double DefaultHeight = 100;
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
            LabelArea = ComputeLabelArea(label, pos);
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
            get => ModelFactoryRegistry.Instance.GetSketchItemFactory().GetAllowableConnctors(this.GetType());
        }

        protected ConnectableBase(SerializationInfo info, StreamingContext context):base(info, context)
        {}

        public override void UpdateGeometry()
        {
            _geometry.Children.Clear();
            _geometry.Children.Add(new RectangleGeometry(Bounds));
            
        }

        public override Geometry Geometry => _geometry;

        public override void RenderAdornments(DrawingContext drawingContext)
        {
            // draw the lable
            if (LabelArea != Rect.Empty)
            {
                RenderLable(drawingContext);
            }
        }

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

        public Rect Bounds
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

        protected virtual FormattedText ComputeFormattedText(
                        string label,  
                        Typeface typeface, double size,
                        FlowDirection flowDirection = System.Windows.FlowDirection.LeftToRight
            )
        {
            return new FormattedText(label, System.Globalization.CultureInfo.CurrentCulture,
                    flowDirection, typeface, size, Stroke,
                    VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip);
        }

        protected virtual Size ComputeFormattedTextSize(string label,
                        Typeface typeface, double size, 
                        double marginX, double marginY)
        {
            var formattedText = ComputeFormattedText(label, typeface, size);
            return new Size(formattedText.Width + marginX,
                formattedText.Height + marginY);
        }


        /// <summary>
        /// provide an interface to populate a toolbar from close to the view!
        /// </summary>
        //public abstract IList<ICommandDescriptor> Tools
        //{
        //    get;
        //}


        protected virtual void RenderLable(DrawingContext drawingContext)
        {
            // draw the lable
            FormattedText t = new FormattedText(Label, System.Globalization.CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight, DefaultFont, _fontSize,
                System.Windows.Media.Brushes.Black,
                VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip);
            var position = new System.Windows.Point(LabelArea.Left + 5, LabelArea.Top + 2);
            drawingContext.DrawText(t, position);
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
                double labelHeight = LabelArea.Height;
                var newLabelArea = LabelArea;
                newLabelArea.Transform(translation.Value);
                newLabelArea.Height = labelHeight;
                LabelArea = newLabelArea;
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

        protected virtual Rect ComputeLabelArea(string label, Point pos)
        {
            if (!string.IsNullOrEmpty(Label))
            {
                return new Rect(
                    new Point(pos.X + 5, pos.Y + 5),
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

        

        protected override void RestoreData(SerializationInfo info, StreamingContext context)
        {
            base.RestoreData(info, context);
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
