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
using Sketch.Helper;
using Sketch.Helper.Binding;
using Sketch.View.CustomControls;
using Sketch.Models;
using Sketch.Interface;

namespace Sketch.View
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

        public static readonly DependencyProperty CanChangeSizeProperty =
            DependencyProperty.Register("CanChangeSize", typeof(bool), typeof(OutlineUI),
            new PropertyMetadata(OnCanChangeSizeChanged));

        public static readonly DependencyProperty ContextMenuDeclarationProperty =
            DependencyProperty.Register("ContextMenuDeclaration", typeof(IList<ICommandDescriptor>), typeof(OutlineUI),
            new PropertyMetadata(OnContextMenuDeclarationChanged));


        Geometry _privateClip;
        bool _isNotifyingSelectionChange;
        bool _isNotifyingMarkingChanged;
        RelativePosition _mouseMoveHitResult;
        
        readonly ConnectableBase _model;
        readonly ISketchItemDisplay _parent;
        readonly SketchPad _sketchPad;
        readonly OutlineAdorner _adorner;
        readonly DecoratorAdorner _decoratorAdorner;
        bool _isShadowAdornderAdded;
        bool _isDecoratorAdornerAdded;
        StackPanel _myTools;
        IEditOperation _currentOperationHandler;
        ISketchItemDisplay _brushDisplay;
        Brush _visualBrush;

        public OutlineUI(SketchPad pad, ISketchItemDisplay parent, object modelInstance)
            :base()
        {
            if (modelInstance is ConnectableBase model)
            {
                _sketchPad = pad;
                _model = model;
                _model.Decorators.CollectionChanged += Decorators_CollectionChanged;
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
                this.SetBinding(LabelProperty, model.LabelPropertyName);

                var boundsBinding = new Binding(nameof(Bounds))
                {
                    Mode = BindingMode.OneWay
                };
                
               

                this.SetBinding(BoundsProperty, boundsBinding);
                this.SetBinding(GeometryProperty, nameof(Geometry));


                this.Visibility = System.Windows.Visibility.Visible;

                // some of the bindings must only occur after the shadow was created!
                _adorner = new OutlineAdorner(this, parent);
                _decoratorAdorner = new DecoratorAdorner(this, parent);
                

                var isSelecteBinding = new Binding("IsSelected") { Mode = BindingMode.TwoWay };
                
                this.SetBinding(IsSelectedProperty, isSelecteBinding);

                var isMarkedBinding = new Binding("IsMarked"){ Mode = BindingMode.TwoWay };
                
                this.SetBinding(IsMarkedProperty, isMarkedBinding);

                this.SetBinding(CanChangeSizeProperty, nameof(CanChangeSize));

                InitContextMenu(model);
                
                
            }
            else
            {
                throw new NotSupportedException("The model needs to be derived from ConnectableBase");
            }
        }

        private void Decorators_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _decoratorAdorner?.InvalidateVisual();
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

        public bool CanChangeSize
        {
            get { return (bool)GetValue(CanChangeSizeProperty); }
            set { SetValue(CanChangeSizeProperty, value); }
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
            
            drawingContext.PushClip(_privateClip);
            _model.RenderAdornments(drawingContext);
            //ClipToBounds = false;
            drawingContext.Pop();
            _adorner.InvalidateVisual();
            _decoratorAdorner.InvalidateVisual();
            
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
            var p = PlacementHelper.RoundToGrid( e.GetPosition(this));
            
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
                    Point p = PlacementHelper.RoundToGrid( e.GetPosition(this._parent.Canvas));
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
                                && _model.CanChangeSize)
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
            _myTools = new StackPanel() { Orientation = Orientation.Vertical };
            
            foreach( var cd in commands)
            {
                _myTools.Children.Add(
                    new ToolbarButton
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
                if (!_isShadowAdornderAdded)
                {
                    adornderlayer.Add(_adorner);
                    _adorner.InvalidateVisual();
                    _isShadowAdornderAdded = true;
                }
                
            }
            else
            {
                if (_isShadowAdornderAdded)
                {
                    adornderlayer.Remove(_adorner);
                    _isShadowAdornderAdded = false;
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

        void InitContextMenu(ConnectableBase model)
        {
            Sketch.Helper.RuntimeCheck.Contract.Requires<ArgumentNullException>(model != null, "InitContextMenu() requires valide model");
            var commands = model.Commands;
            if (commands != null && commands.Count > 0)
            {

                ContextMenu = OutlineHelper.InitContextMenu(commands);
                if (_model.CanCopy && _model.IsSerializable)
                {
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
                if (_model.CanChangeZorder)
                {

                    var z = Canvas.GetZIndex(this);
                    ContextMenu.Items.Add(
                        new MenuItem()
                        {
                            Header = "Bring to front",
                            Command = new DelegateCommand(() =>
                            {
                                GetMinMaxZ(out int minZ, out int maxZ);
                                Canvas.SetZIndex(this, maxZ + 1);
                            })
                        });
                    ContextMenu.Items.Add(
                        new MenuItem()
                        {
                            Header = "Send to back",
                            Command = new DelegateCommand(() =>
                            {
                                GetMinMaxZ(out int minZ, out int maxZ);
                                Canvas.SetZIndex(this, minZ - 1);
                            })
                        });
                }
            }

        }

        void GetMinMaxZ(out int minZ, out int maxZ)
        {
            minZ = int.MaxValue;
            maxZ = int.MinValue;
            foreach (var sibling in this._parent.Canvas.Children.OfType<ISketchItemUI>())
            {
                if (this.Bounds.IntersectsWith(sibling.Model.Geometry.Bounds))
                {
                    var otherZ = Canvas.GetZIndex(sibling.Shape);
                    if (otherZ > maxZ) maxZ = otherZ;
                    if (otherZ < minZ) minZ = otherZ;
                }
            }
        }

        private static void OnLabelChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {
            if( source is OutlineUI outlineUI) outlineUI.Model.UpdateGeometry();

        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            var layer = AdornerLayer.GetAdornerLayer(this);
            if( layer != null && !_isDecoratorAdornerAdded)
            {
                layer.Add(_decoratorAdorner);
                _isDecoratorAdornerAdded = true;
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
                if (gadgetUI._isShadowAdornderAdded)
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

        private static void OnCanChangeSizeChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {

            OutlineUI gadget = source as OutlineUI;
            if (e.NewValue != e.OldValue)
            {
                if (gadget.CanChangeSize)
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

     
        public void Dispose(){}
    }
}
