using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UI.Utilities.Interfaces;
using System.Runtime.Serialization;
using Prism.Commands;
using UI.Utilities;
using Sketch.Interface;

namespace Sketch.Models
{

    public abstract class ConnectableBase: ModelBase, IBoundedItemModel
    {
        

        Rect _bounds;
        bool _allowSizeChange;
        double _rotationAngle = 0.0;
        RotateTransform _rotateTransform = null;
        SerializableColor _fillColor = new SerializableColor();
        Color _initialColor = Colors.Wheat;

        DelegateCommand _cmdSelectColor;

        public ConnectableBase() 
        {
            FillColor = Colors.Wheat;
            Initialize();
        }

        public virtual IList<ICommandDescriptor> AllowableConnectors
        {
            get => ModelFactoryRegistry.Instance.GetSketchItemFactory().GetAllowableConnctors(this.GetType());
        }

        protected ConnectableBase(SerializationInfo info, StreamingContext context):base(info, context)
        {   
            _fillColor = new SerializableColor();
            _bounds = (Rect)info.GetValue("Bounds", typeof(Rect));
            _allowSizeChange = info.GetBoolean("AllowSizeChange");
            try{
               _fillColor.ScRgb =  (float[])info.GetValue("Fill", typeof(float[]));
            }
            catch { }

            Initialize();
        }

        public override void RenderAdornments(DrawingContext drawingContext)
        {
            FormattedText t = new FormattedText(Name, System.Globalization.CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Black);
            var position = new Point(LabelArea.Left + 5, LabelArea.Top + 2);
            
            drawingContext.DrawText(t, position);
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
                RaisePropertyChanged("Geometry");
            }
        }

        public bool AllowSizeChange
        {
            get { return _allowSizeChange; }
            set { SetProperty<bool>(ref _allowSizeChange, value); }
        }

        public abstract RectangleGeometry Outline
        {
            get;
        }

        /// <summary>
        /// provide an interface to populate a toolbar from close to the view!
        /// </summary>
        //public abstract IList<ICommandDescriptor> Tools
        //{
        //    get;
        //}

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
            
            NotifyShapeChanged( ref oldBounds, ref newBounds);
        }

        public event EventHandler<OutlineChangedEventArgs> ShapeChanged;

        protected virtual void NotifyShapeChanged(ref Rect old, ref Rect @new)
        {
            if( ShapeChanged != null)
            {
                ShapeChanged( this, new OutlineChangedEventArgs(old,@new));
            }
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("AllowSizeChange", AllowSizeChange);
            info.AddValue("Bounds", Bounds);
            info.AddValue("Fill", _fillColor.ScRgb );
        }

        #region private helpers
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

        void Initialize()
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
