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
    public partial class OutlineUI: Shape, ISketchItemUI
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

        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(OutlineUI),
            new PropertyMetadata(OnGeometryChanged));

        public static readonly DependencyProperty AllowSizeChangeProperty =
            DependencyProperty.Register("AllowSizeChange", typeof(bool), typeof(OutlineUI),
            new PropertyMetadata(OnAllowSizeChangeChanged));

        public static readonly DependencyProperty ContextMenuDeclarationProperty =
            DependencyProperty.Register("ContextMenuDeclaration", typeof(IList<ICommandDescriptor>), typeof(OutlineUI),
            new PropertyMetadata(OnContextMenuDeclarationChanged));


        

        Geometry _geometry;
        bool _isNotifyingSelectionChange;
        bool _isNotifyingMarkingChanged;
        Point _lastMouseDown; // this is used to show the tools if the model allows this
        Models.ConnectableBase _model;
        ISketchItemDisplay _parent;
        OutlineAdorner _adorner;
        bool _isAdornderAdded;
        StackPanel _myTools;
        IEditOperation _currentOperationHandler;

        public OutlineUI(ISketchItemDisplay parent, ConnectableBase model)
            :base()
        {
            _model = model;
            _parent = parent;
            var tools = model.AllowableConnectors;
            if (tools.Count > 0)
            {
                InitTools(tools);
            }

            this.DataContext = model;
            this.SetBinding(BoundsProperty, "Bounds");
            this.SetBinding(GeometryProperty, "Geometry");
            
            this.Visibility = System.Windows.Visibility.Visible;


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

        void HandleShadowMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released)
            {
                if (_currentOperationHandler != null)
                {
                    _currentOperationHandler.StopOperation(true);
                }
                RegisterHandler(new ChangeSizeOperation(this, e.GetPosition(_parent.Canvas)));
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


        public Shape Shape
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

        public void StartEdit()
        {
            var editOperation = new EditLabelOperation(this); 
            RegisterHandler(editOperation);
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

        protected override Geometry DefiningGeometry
        {
            get { return Geometry; }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            _parent.HandleKeyDown(e);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.PushClip(_model.Outline);

            if (!_model.Render(drawingContext))
            {
                base.OnRender(drawingContext);
            }
            _model.RenderAdornments(drawingContext);
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
            var p = e.GetPosition(this._parent.Canvas);
            
            if (_currentOperationHandler == null)
            {
                e.Handled = true;
                if (LabelArea.Contains(p) && Model.AllowEdit)
                {
                    Mouse.OverrideCursor = Cursors.Hand;
                }
                else if (e.LeftButton == MouseButtonState.Released &&
                    e.RightButton == MouseButtonState.Released)
                {
                    Mouse.OverrideCursor = Cursors.SizeAll;
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
                if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released)
                {
                    Point p = e.GetPosition(this._parent.Canvas);
                    _lastMouseDown = p;
                    if (IsSelected)
                    {
                        if (LabelArea.Contains(p) && this.Model.AllowEdit )
                        {
                            RegisterHandler(new EditLabelOperation(this));
                            return;
                        }
                    }
                    // toggle on mouse press
                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        IsMarked = !IsMarked;
                        //NotifyIsMarkedChanged();
                    }
                    else
                    {
                        IsSelected = true;
                        //NotifySelectionChanged();
                        if (_adorner.HitShadowBorder(p) != RelativePosition.Undefined
                            && _model.AllowSizeChange )
                        {
                            RegisterHandler(new ChangeSizeOperation(this, p));
                            return;
                        }
                        RegisterHandler(new MoveOperation(this, p));
                        
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
                    
                    var pos = Bounds.TopRight;
                    Canvas.SetTop(_myTools, pos.Y + 2);
                    Canvas.SetLeft(_myTools, pos.X - 2);
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

        private static void OnSelectedChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {
            
            OutlineUI outlineUI = source as OutlineUI;
            if (e.NewValue != e.OldValue)
            {
                outlineUI.NotifySelectionChanged();
                outlineUI.ShowHideTools();
                outlineUI.AddRemoveAdorner();
                if (outlineUI._currentOperationHandler is EditLabelOperation)
                {
                    outlineUI._currentOperationHandler.StopOperation(true);
                }
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
                if (outlineUI._currentOperationHandler is EditLabelOperation)
                {
                    outlineUI._currentOperationHandler.StopOperation(true);
                }
            }
        }

        private static void OnBoundsChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {
            OutlineUI gadgetUI = source as OutlineUI;
            if (e.NewValue != e.OldValue)
            {
                gadgetUI.InvalidateVisual();
                gadgetUI.ShowHideTools();
                if (gadgetUI._isAdornderAdded)
                {
                    gadgetUI._adorner.UpdateGeometry();
                }
            }
        }


        private static void OnGeometryChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {
            OutlineUI gadgetUI = source as OutlineUI;
            if (e.NewValue != e.OldValue)
            {
                gadgetUI._geometry = (Geometry)e.NewValue;
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

    }
}
