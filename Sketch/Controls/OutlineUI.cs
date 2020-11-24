using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using UI.Utilities.Controls;
using UI.Utilities.Interfaces;
using Sketch.Types;
using Sketch.Models;
using Sketch.Interface;
using Prism.Commands;

namespace Sketch.Controls
{
    public partial class OutlineUI: ContentControl, ISketchItemUI
    {
        
        public static readonly DependencyProperty BoundsProperty =
            DependencyProperty.Register("Bounds", typeof(Rect), typeof(OutlineUI),
            new PropertyMetadata(OnBoundsChanged));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(OutlineUI),
            new PropertyMetadata(OnSelectedChanged));

        public static readonly DependencyProperty IsMarkedProperty =
            DependencyProperty.Register("IsMarked", typeof(bool), typeof(OutlineUI),
            new PropertyMetadata(OnIsMarkedChanged));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(OutlineUI),
            new PropertyMetadata(OnLabelChanged));

        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(OutlineUI),
            new PropertyMetadata(OnGeometryChanged));

        public static readonly DependencyProperty AllowSizeChangeProperty =
            DependencyProperty.Register("AllowSizeChange", typeof(bool), typeof(OutlineUI),
            new PropertyMetadata(OnAllowSizeChangeChanged));

        public static readonly DependencyProperty ContextMenuDeclarationProperty =
            DependencyProperty.Register("ContextMenuDeclaration", typeof(IList<ICommandDescriptor>), typeof(OutlineUI),
            new PropertyMetadata(OnContextMenuDeclarationChanged));


        Geometry _privateClip;
        bool _isNotifyingSelectionChange;
        bool _isNotifyingMarkingChanged;
        RelativePosition _mouseMoveHitResult;
        
        Models.ConnectableBase _model;
        ISketchItemDisplay _parent;
        SketchPad _sketchPad;
        OutlineAdorner _adorner;
        bool _isAdornderAdded;
        StackPanel _myTools;
        IEditOperation _currentOperationHandler;
        ISketchItemDisplay _brushDisplay;
        Brush _visualBrush;

        public OutlineUI(SketchPad pad, ISketchItemDisplay parent, ConnectableBase model)
            :base()
        {
            _sketchPad = pad;
            _model = model;
            Content = model;
            IsHitTestVisible = true;
            Canvas.SetLeft(this, model.Bounds.Left);
            Canvas.SetTop(this, model.Bounds.Top);
            _parent = parent;
            var tools = model.AllowableConnectors;
            if (tools.Count > 0)
            {
                InitTools(tools);
            }

            this.DataContext = model;
            this.SetBinding(LabelProperty, model.LabelPropertyName );

            var boundsBinding = new Binding(nameof(Bounds));
            boundsBinding.Mode = BindingMode.OneWay;
            //boundsBinding.IsAsync = true;
            
            this.SetBinding(BoundsProperty, boundsBinding) ;
            this.SetBinding(GeometryProperty, nameof(Geometry));
            
            
            this.Visibility = System.Windows.Visibility.Visible;

            /* commented

            // allow to bind Fill property
            if (model.GetType().GetProperty("Fill") != null)
            {
                this.SetBinding(FillProperty, "Fill");
            }
            else
            {
                Fill = Brushes.Wheat;
            }

            // allow to bind Stroke property
            if( model.GetType().GetProperty("Stroke") != null)
            {
                this.SetBinding(StrokeProperty, "Stroke");
            }
            else
            {
                Stroke = Brushes.Black;
            }

            // allow to bind Stroke Thickness property
            if (model.GetType().GetProperty("StrokeThickness") != null)
            {
                this.SetBinding(StrokeThicknessProperty, "StrokeThickness");
            }
            else
            {
                this.StrokeThickness = 0.1;
            }

            // allow to bind Stroke dash array property
            if (model.GetType().GetProperty("StrokeDashArray") != null)
            {
                this.SetBinding(StrokeDashArrayProperty, "StrokeDashArray");
            }
            */

            // some of the bindings must only occur after the shadow was created!
            _adorner = new OutlineAdorner(this, parent);

            var isSelecteBinding = new Binding("IsSelected");
            isSelecteBinding.Mode = BindingMode.TwoWay;
            this.SetBinding(IsSelectedProperty, isSelecteBinding);

            var isMarkedBinding = new Binding("IsMarked");
            isMarkedBinding.Mode = BindingMode.TwoWay;
            this.SetBinding(IsMarkedProperty, isMarkedBinding);

            this.SetBinding(AllowSizeChangeProperty, "AllowSizeChange");

            var commands = model.Commands;
            if( commands != null && commands.Count > 0)
            {

                ContextMenu = OutlineHelper.InitContextMenu(commands);
                ContextMenu.Items.Add(
                    new MenuItem()
                    {
                        Header = "Copy",
                        Command = new DelegateCommand(() =>
                        {
                            var newElem = _model.Clone();
                            var t = new TranslateTransform(
                                ConnectableBase.DefaultWidth / 4,
                                ConnectableBase.DefaultHeight / 4);
                            newElem.Move(t);
                            _parent.SketchItems.Add(newElem);
                        })
                    });
            }
        }

        public Brush VisualBrush
        {
            get 
            {
                if( _visualBrush == null )
                {
                    _visualBrush = CreateVisualBrush();
                }
                return _visualBrush;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            //ContentTemplate = FindResource("UmlState") as DataTemplate;
        }

        void HandleShadowMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released)
            {
                if (_currentOperationHandler != null)
                {
                    _currentOperationHandler.StopOperation(true);
                }
                RegisterHandler(new ChangeSizeOperation(this, e) );
                e.Handled = true;
            } 
        }


        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set 
            { 
                SetValue(IsSelectedProperty, value);
                IsMarked = value;
            }
        }

        public bool IsMarked
        {
            get { return (bool)GetValue(IsMarkedProperty); }
            set { SetValue(IsMarkedProperty, value); }
        }

        public Rect Bounds
        {
            get { return (Rect)GetValue(BoundsProperty); }
            set { SetValue(BoundsProperty, value); }
        }

        public Geometry Geometry
        {
            get { return (Geometry)GetValue(GeometryProperty); }
            set { SetValue(GeometryProperty, value); }
        }

        public bool AllowSizeChange
        {
            get { return (bool)GetValue(AllowSizeChangeProperty); }
            set { SetValue(AllowSizeChangeProperty, value); }
        }


        public UIElement Shape
        {
            get { return this; }
        }

        public Rect LabelArea
        {
            get { return _model.LabelArea; }
        }

        public void Disable()
        {
            RegisterHandler(new NopHandler(_parent));
        }

        public void Enable()
        {
            RegisterHandler(null);
        }

        public void TriggerSnapshot()
        {
            _parent.TakeSnapshot();
        }

        public void DropSnapshot()
        {
            _parent.DropSnapshot();
        }

        public ISketchItemModel Model
        {
            get { return _model; }
        }

        public ISketchItemModel RefModel
        {
            get
            {
                return _model.RefModel;
            }
        }

        

        private void RegisterHandler(IEditOperation handler)
        {
            var oldHandler = _currentOperationHandler;
            _currentOperationHandler = handler;
            if (oldHandler != null)
            {
                oldHandler.StopOperation(false);
            }
        }

        public event EventHandler<bool> SelectionChanged;
        public event EventHandler<bool> IsMarkedChanged;

        //protected /*override*/ Geometry DefiningGeometry
        //{
        //    get { return Geometry; }
        //}

        protected override void OnKeyDown(KeyEventArgs e)
        {
            _parent.HandleKeyDown(e);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (!_model.Render(drawingContext))
            {
                base.OnRender(drawingContext);
            }
            var r = _model.Bounds;
           
            drawingContext.PushClip(_privateClip);
            _model.RenderAdornments(drawingContext);
            //ClipToBounds = false;
            drawingContext.Pop();
            _adorner.InvalidateVisual();
            
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            e.Handled = false;
            if( _currentOperationHandler == null)
            {
                Mouse.OverrideCursor = null;
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            e.Handled = false;
            var p = RoundToGrid( e.GetPosition(this));
            
            if (_currentOperationHandler == null)
            {
                e.Handled = true;
                //if (LabelArea.Contains(p) && Model.AllowEdit)
                //{
                //    Mouse.OverrideCursor = Cursors.Hand;
                //}
                //else 
                if (e.LeftButton == MouseButtonState.Released &&
                    e.RightButton == MouseButtonState.Released)
                {

                    _mouseMoveHitResult = RelativePosition.Undefined;
                    if (IsSelected)
                    {
                        _mouseMoveHitResult = _adorner.HitShadowBorder(p);
                    }
                    SetMouseCursor(_mouseMoveHitResult);
                    PositionTools();
                }
            }
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            _parent.Canvas.Focus();
            if (_currentOperationHandler != null)
            {
                //_currentOperationHandler.OnMouseDown(e);
                return;
            }
            else
            {
                e.Handled = true;
                // only react on left button
                if (e.LeftButton == MouseButtonState.Pressed 
                    && e.RightButton == MouseButtonState.Released)
                {
                    Point p = RoundToGrid( e.GetPosition(this._parent.Canvas));
                    var hitResult = _adorner.HitShadowBorder(p);
                    SetMouseCursor(hitResult);
                    // toggle on mouse press
                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        IsMarked = !IsMarked;
                        
                    }
                    else
                    {
                        if (hitResult == _mouseMoveHitResult)
                        {
                            if (_mouseMoveHitResult != RelativePosition.Undefined
                                && _model.AllowSizeChange)
                            {
                                RegisterHandler(new ChangeSizeOperation(this, e));

                            }
                            else
                            {
                                RegisterHandler(new MoveOperation(this, p));
                            }
                        }
                        IsSelected = true;
                    }
                }
            }
        }

        void InitTools( IEnumerable<ICommandDescriptor> commands)
        {
            _myTools = new StackPanel();
            _myTools.Orientation = Orientation.Vertical;
            foreach( var cd in commands)
            {
                _myTools.Children.Add(
                    new UI.Utilities.Controls.ToolbarButton
                    {
                        ImageBitmap = cd.Bitmap,
                        Command = cd.Command,
                        ToolTip = cd.Name,
                        Width = 16,
                        Height = 16,
                        BorderBrush = Brushes.DarkGray,
                        Style = (System.Windows.Style)this.TryFindResource(ToolBar.ButtonStyleKey)
                    }
                    );
            }
            
        }

        void ShowHideTools()
        {
            if( IsSelected)
            {
                if( _myTools != null)
                {
                    if( !_parent.Canvas.Children.Contains(_myTools))
                    {
                        _parent.Canvas.Children.Add(_myTools);
                        Canvas.SetZIndex(_myTools, 500);
                    }
                    PositionTools();
                    _myTools.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                if( _myTools != null)
                {
                    _myTools.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }

        internal void PositionTools()
        {
            if (_myTools != null)
            {
                var p = Mouse.GetPosition(_parent.Canvas);
                var pos = _model.GetPreferredToolsLocation(p, out ConnectorDocking docking);
                if (((int)docking & (int)ConnectorDocking.Bottom) != 0)
                {
                    _myTools.Orientation = Orientation.Horizontal;
                }
                else if (((int)docking & (int)ConnectorDocking.Top) != 0)
                {
                    _myTools.Orientation = Orientation.Horizontal;
                    pos.Y = pos.Y -= 16;
                }
                else if (((int)docking & (int)ConnectorDocking.Left) != 0)
                {
                    _myTools.Orientation = Orientation.Vertical;
                    pos.Y = pos.Y += 2;
                    pos.X = pos.X -= 20;
                }
                else
                {
                    _myTools.Orientation = Orientation.Vertical;
                    pos.Y = pos.Y += 2;
                    pos.X = pos.X -= 2;
                }
                Canvas.SetTop(_myTools, pos.Y);
                Canvas.SetLeft(_myTools, pos.X);
            }
        }

        void NotifySelectionChanged()
        {
            // avoid infinite recursion!
            if(SelectionChanged != null && !_isNotifyingSelectionChange)
            {
                _isNotifyingSelectionChange = true;
                SelectionChanged(this, this.IsSelected);
            }
            _isNotifyingSelectionChange = false;
        }

        void NotifyIsMarkedChanged()
        {
            if( IsMarkedChanged != null && !_isNotifyingMarkingChanged)
            {
                _isNotifyingMarkingChanged = true;
                IsMarkedChanged(this, this.IsMarked);
            }
            _isNotifyingMarkingChanged = false;
        }

        void AddRemoveAdorner()
        {
            var adornderlayer = AdornerLayer.GetAdornerLayer(this);
            if (adornderlayer == null) return;
            
            if (IsSelected || IsMarked)
            {
                if (!_isAdornderAdded)
                {
                    adornderlayer.Add(_adorner);
                    _adorner.InvalidateVisual();
                    _isAdornderAdded = true;
                }
                
            }
            else
            {
                if (_isAdornderAdded)
                {
                    adornderlayer.Remove(_adorner);
                    _isAdornderAdded = false;
                }
                
            }
        }

        void SetMouseCursor(RelativePosition d)
        {
            switch(d)
            {
                case RelativePosition.E:
                case RelativePosition.W:
                    Mouse.OverrideCursor = Cursors.SizeWE;
                    break;
                case RelativePosition.N:
                case RelativePosition.S:
                    Mouse.OverrideCursor = Cursors.SizeNS;
                    break;
                case RelativePosition.SE:
                case RelativePosition.NW:
                    Mouse.OverrideCursor = Cursors.SizeNWSE;
                    break;
                case RelativePosition.NE:
                case RelativePosition.SW:
                    Mouse.OverrideCursor = Cursors.SizeNESW;
                    break;
                default:
                    Mouse.OverrideCursor = Cursors.SizeAll;
                    break;
            }
            
        }

        private Brush CreateVisualBrush()
        {
            if( _model is ISketchItemContainer container )
            {
                _brushDisplay = new SketchItemDisplay(_sketchPad, container, false);
                _visualBrush = new VisualBrush(_brushDisplay.Canvas);
            }
            else
            {
                _visualBrush = new VisualBrush(
                    new GeometryBorder()
                    { 
                        BorderThickness = new Thickness(1,1,1,1),
                        BorderBrush = _model.Stroke,
                        Background = _model.Fill,
                        BorderGeometry = _model.Geometry }
                    );
            }
            return _visualBrush;
        }

        private static void OnLabelChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {
            if( source is OutlineUI outlineUI) outlineUI.Model.UpdateGeometry();

        }


        private static void OnSelectedChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {
            
            OutlineUI outlineUI = source as OutlineUI;
            if (e.NewValue != e.OldValue)
            {
                outlineUI.NotifySelectionChanged();
                outlineUI.ShowHideTools();
                outlineUI.AddRemoveAdorner();
            }
        }

        private static void OnIsMarkedChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            OutlineUI outlineUI = source as OutlineUI;
            if (e.NewValue != e.OldValue)
            {
                outlineUI.NotifyIsMarkedChanged();
                outlineUI.ShowHideTools();
                outlineUI.AddRemoveAdorner();
            }
        }

        private static void OnBoundsChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {
            OutlineUI gadgetUI = source as OutlineUI;
            if (e.NewValue != e.OldValue)
            {
                var r = (Rect)e.NewValue;
                
                gadgetUI.ShowHideTools();
                if (gadgetUI._isAdornderAdded)
                {
                    gadgetUI._adorner.UpdateGeometry();
                }

                var loc = r.Location;

                Canvas.SetLeft(gadgetUI, loc.X);
                Canvas.SetTop(gadgetUI, loc.Y);
                gadgetUI.Height = r.Height;
                gadgetUI.Width = r.Width;
                gadgetUI._privateClip = new RectangleGeometry(new Rect(r.Size));
            }
            gadgetUI.Model?.UpdateGeometry();
            gadgetUI.InvalidateVisual();
            gadgetUI.UpdateLayout();
        }


        private static void OnGeometryChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {
            OutlineUI gadgetUI = source as OutlineUI;
            if (e.NewValue != e.OldValue)
            {
                gadgetUI.InvalidateVisual();
                gadgetUI.ShowHideTools();
                if (gadgetUI._adorner != null)
                {
                    gadgetUI._adorner.InvalidateVisual();
                }
            }
            gadgetUI.Shape.InvalidateVisual();
        }

        private static void OnAllowSizeChangeChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {

            OutlineUI gadget = source as OutlineUI;
            if (e.NewValue != e.OldValue)
            {
                if (gadget.AllowSizeChange)
                {
                    gadget._adorner.MouseDown += gadget.HandleShadowMouseDown;
                }
                else
                {
                    gadget._adorner.MouseDown -= gadget.HandleShadowMouseDown;
                }
                gadget._adorner.SetEnableResizeOperation((bool)e.NewValue);
            }
        }

        private static void OnContextMenuDeclarationChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {

        }

        internal static Point RoundToGrid(Point p)
        {
            return new Point(RoundToGrid(p.X), RoundToGrid(p.Y));
        }

        internal static double RoundToGrid(double val)
        {
            return Math.Round(val / SketchPad.GridSize) * SketchPad.GridSize;
        }

        public void Dispose(){}
    }
}
