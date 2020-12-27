
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Sketch.Helper.Binding;
using Sketch.Models;
using Sketch.Helper;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Sketch.Interface;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

namespace Sketch.View
{
    public partial class ConnectorUI: Shape, ISketchItemUI
    {
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(ConnectorUI),
            new PropertyMetadata(OnGeometryChanged));

        public static readonly DependencyProperty IsSelectedPropery =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ConnectorUI),
            new PropertyMetadata(OnSelectedChanged));

        public static readonly DependencyProperty IsMarkedPropery =
            DependencyProperty.Register("IsMarked", typeof(bool), typeof(ConnectorUI),
            new PropertyMetadata(OnIsMarkedChanged));

        public static readonly DependencyProperty ConnectorLineStyleProperty =
            DependencyProperty.Register("ConnectorLineStyle", typeof(LineStyle), typeof(ConnectorUI),
            new PropertyMetadata(OnConnectorLineStyleChanged));

        public static readonly DependencyProperty ConnectorLineWidthProperty =
            DependencyProperty.Register("ConnectorLineWidth", typeof(double), typeof(ConnectorUI),
            new PropertyMetadata(OnConnectorLineWidthChanged));

        public static readonly DependencyProperty ConnectorStartLabelProperty =
            DependencyProperty.Register("ConnectorStartLabel", typeof(ConnectorLabelModel), typeof(ConnectorUI),
            new PropertyMetadata(OnConnectorStartLabelChanged));

        public static readonly DependencyProperty ConnectorEndLabelProperty =
            DependencyProperty.Register("ConnectorEndLabel", typeof(ConnectorLabelModel), typeof(ConnectorUI),
            new PropertyMetadata(OnConnectorEndLabelChanged));

        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(ConnectableBase), typeof(ConnectorUI),
            new PropertyMetadata(OnFromChanged));

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(ConnectableBase), typeof(ConnectorUI),
            new PropertyMetadata(OnToChanged));

        public static readonly DependencyProperty ContextMenuDeclarationProperty =
            DependencyProperty.Register("ContextMenuDeclaration", typeof(IList<ICommandDescriptor>), typeof(ConnectorUI),
            new PropertyMetadata(OnContextMenuDeclarationChanged));

        public static readonly DependencyProperty WaypointsProperty =
            DependencyProperty.Register("Waypoints", typeof(ObservableCollection<IWaypoint>), typeof(ConnectorUI),
                new PropertyMetadata(OnWaypointsChanged));


       
        readonly ConnectorModel _model;
        readonly ISketchItemDisplay _parent;
        Geometry _myGeometry = null;
        IEditOperation _currentOperationHandler;
        int _lineWidth;
        Brush _lineBrush;
        readonly Pen _linePen = new Pen();
        static readonly Brush _selectedLineBrush = new SolidColorBrush(Colors.Blue) { Opacity = 0.5 };
        static readonly Pen _hitTestPen = new Pen() { Thickness = 20 };

        readonly ConnectorAdorner _myAdorner;
        
        bool _addornerAdded = false;
        Point _lastContextMenuPosition;
        MenuItem _deleteWaypointMenuItem;

        public ConnectorUI(ISketchItemDisplay parent, object modelInstance )
        {
            if (modelInstance is ConnectorModel model)
            {
                _model = model;
                _parent = parent;
                this.DataContext = _model;

                _myAdorner = new ConnectorAdorner(_parent, this);
                _myAdorner.MouseLeftButtonDown += Adorner_MouseLeftButtonDown;
                _myAdorner.MouseRightButtonDown += Adorner_MouseRightButtonDown;

                var isSelectedBinding = new Binding("IsSelected")
                {
                    Mode = BindingMode.TwoWay
                };

                this.SetBinding(IsSelectedPropery, isSelectedBinding);

                var startLabelBinding = new Binding("ConnectorStartLabel") { Mode = BindingMode.OneWay };

                this.SetBinding(ConnectorStartLabelProperty, startLabelBinding);

                var endLabelBinding = new Binding("ConnectorEndLabel") { Mode = BindingMode.OneWay };
                this.SetBinding(ConnectorEndLabelProperty, endLabelBinding);

                var contextMenuBinding = new Binding("ContextMenuDeclaration") { Mode = BindingMode.OneWay };

                this.SetBinding(ContextMenuDeclarationProperty, contextMenuBinding);

                var fromBinding = new Binding("From") { Mode = BindingMode.OneWay };

                this.SetBinding(FromProperty, fromBinding);

                var toBinding = new Binding("To") { Mode = BindingMode.OneWay };

                this.SetBinding(ToProperty, toBinding);

                SetBinding(StrokeDashArrayProperty,
                    new Binding("StrokeDashArray"));

                var waypointBinding = new Binding("Waypoints") { Mode = BindingMode.OneWay };
                SetBinding(WaypointsProperty, waypointBinding);

                _model.From.ShapeChanged += From_ShapeChanged;
                _model.To.ShapeChanged += To_ShapeChanged;

                this.Visibility = System.Windows.Visibility.Visible;
                this.Stroke = Brushes.White;
                this.Fill = Brushes.White;


                this.StrokeThickness = 1;// ComputeConnectorLine.LineWidth;
                _myGeometry = _model.Geometry;

                TryAddAdorner();
                this.SetBinding(GeometryProperty, "Geometry");
            }
            else
            {
                throw new NotSupportedException("Model needs to be derived from class ConnectorModel");
            }
        }

        private void Adorner_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.OnMouseLeftButtonDown(e);
        }

        private void Adorner_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.OnMouseRightButtonDown(e);
            if (ContextMenu != null)
            {
                if (_myAdorner.HitWaypoint(out int index))
                {
                    _deleteWaypointMenuItem = new MenuItem()
                    {
                        Header = "Delete Waypoint",
                        Command = new DelegateCommand(() =>
                        {
                            Waypoints.RemoveAt(index);
                            _model.UpdateGeometry();
                        }
                        )
                    };
                    ContextMenu.Items.Add(_deleteWaypointMenuItem);
                }
                this.ContextMenu.IsOpen = true;
                _lastContextMenuPosition = e.GetPosition(_parent.Canvas);
                this.ContextMenu.Closed += ContextMenu_Closed;

            }
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            if (e.Source is ContextMenu contextMenu)
            {
                contextMenu.Closed -= ContextMenu_Closed;
                contextMenu.Items.Remove(_deleteWaypointMenuItem);
            }
        }


        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedPropery); }
            set { 
                SetValue(IsSelectedPropery, value);
                IsMarked = value;
            }
        }

        public bool IsMarked
        {
            get { return (bool)GetValue(IsMarkedPropery); }
            set { SetValue(IsMarkedPropery, value); }
        }

        public ObservableCollection<IWaypoint> Waypoints
        {
            get { return (ObservableCollection<IWaypoint>)GetValue(WaypointsProperty); }
            set { SetValue(WaypointsProperty, value); }
        }

        public IList<ICommandDescriptor> ContextMenuDeclaration
        {
            get { return (IList<ICommandDescriptor>)GetValue(ContextMenuDeclarationProperty); }
            set { SetValue(ContextMenuDeclarationProperty, value); }
        }

        public ConnectorLabelModel ConnectorStartLabel
        {
            get { return (ConnectorLabelModel)GetValue(ConnectorStartLabelProperty); }
            set { SetValue(ConnectorStartLabelProperty, value); }
        }

        public ConnectorLabelModel ConnectorEndLabel
        {
            get { return (ConnectorLabelModel)GetValue(ConnectorEndLabelProperty); }
            set { SetValue(ConnectorEndLabelProperty, value); }
        }

        public ConnectableBase From
        {
            get { return (ConnectableBase)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public ConnectableBase To
        {
            get { return (ConnectableBase)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public double LineWidth
        {
            get
            {
                return _lineWidth;
            }
        }

        public void TriggerSnapshot()
        {
            _parent.TakeSnapshot();
        }

        public void DropSnapshot()
        {
            _parent.DropSnapshot();
        }

        protected override System.Windows.Media.Geometry DefiningGeometry
        {
            get { return _myGeometry; }
        }

        public Brush LineBrush
        {
            get
            {
                return _lineBrush;
            }
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            bool hit = this.Model.Geometry.StrokeContains(_hitTestPen, hitTestParameters.HitPoint);
            if (hit)
            {
                return new PointHitTestResult(this, hitTestParameters.HitPoint);
            }
            return null;
        }

        public UIElement Shape
        {
            get { return this; }
        }


        public Rect LabelArea
        {
            get { throw new NotImplementedException(); }
        }

        public void Disable(){}

        public void Enable(){}

        public event EventHandler<bool> SelectionChanged;
        public event EventHandler<bool> IsMarkedChanged;

        public ISketchItemModel Model
        {
            get { return _model; }
        }

        public ISketchItemModel RefModel
        {
            get { return _model.RefModel; }
        }

        
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
        
            _parent.Canvas.Focus();
            
            if( _currentOperationHandler != null )
            {
                _currentOperationHandler.StopOperation(false);
                _currentOperationHandler = null;
                return;
            }
            
            e.Handled = true;
            
            var p = e.GetPosition(_parent.Canvas);

            if (_myAdorner.HitWaypoint(out int waypointIndex))
            {
                _currentOperationHandler = new WaypointMoveOperation(this, p, Waypoints[waypointIndex]);
            }
            //else if( Keyboard.IsKeyDown(Key.LeftShift))
            //{
                
            //}
            else
            {
                _currentOperationHandler = new MoveConnectorOperation(this, p);
            }

            if( IsSelected)
            {
                NotifySelectionChanged();
                return;
            }
            IsSelected = true;
            
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            _lastContextMenuPosition = Mouse.GetPosition(_parent.Canvas);
            
            base.OnContextMenuOpening(e);
        }

        protected override void OnContextMenuClosing(ContextMenuEventArgs e)
        {
            base.OnContextMenuClosing(e);
            if ( _deleteWaypointMenuItem != null)
            {
                ContextMenu.Items.Remove(_deleteWaypointMenuItem);
            }
            
        }



        void OnShadowMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _lastContextMenuPosition = e.GetPosition(_parent.Canvas);
            base.OnMouseRightButtonDown(e);
            
        }

        void OnShadowMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnMouseLeftButtonDown(e);
        }

        void TryAddAdorner()
        {
            if (!_addornerAdded)
            {
                var adornderlayer = AdornerLayer.GetAdornerLayer(this);
                if (adornderlayer == null) return;

                adornderlayer.Add(_myAdorner);
                _addornerAdded = true;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            
            
            base.OnRender(drawingContext);
            foreach( var r in _model.Endings )
            {
                drawingContext.DrawGeometry(Fill, _linePen, r.Ending);
            }
            _model.RenderAdornments(drawingContext);
        }

        private void  UpdateGeometry()
        {
            TryAddAdorner();
            _model.UpdateGeometry();
            _parent.ShowIntersections();
            _myAdorner.InvalidateVisual();
        }

        void To_ShapeChanged(object sender, OutlineChangedEventArgs e)
        {
            // make a quick plausibility test in order to avoid overlapping of connector and shape!
            CheckLineEnding();
            UpdateGeometry();
            
        }

        void From_ShapeChanged(object sender, OutlineChangedEventArgs e)
        {
            // make a quick plausibility test in order to avoid overlapping of connector and shape!
            CheckLineStart();
            UpdateGeometry();
        }

        void CheckLineStart()
        {
            if (_model.From == null || _model.To == null) return;

            // nothing to do for self transistions!
            if (_model.From == _model.To) return;

            _model.Reset();
            var startingFrom = _model.Current;
            _model.MoveNext();
            var endingAt = _model.Current;
            CheckLineConfiguration(startingFrom, endingAt);
        }

        void CheckLineEnding()
        {
            if (_model.From == null || _model.To == null) return;

            // nothing to do for self transistions!
            if (_model.From == _model.To) return;

            _model.Reset(); // reset the iterator
            var startingFrom = _model.Current;
            while (_model.MoveNext()) startingFrom = _model.Current;
            var endingAt = _model.Current;

            CheckLineConfiguration(startingFrom, endingAt);
        }

        void CheckLineConfiguration(IWaypoint from, IWaypoint to)
        {
            var startPoint = ConnectorUtilities.ComputePoint(from.Bounds, from.OutgoingDocking, from.OutgoingRelativePosition);
            var endPoint = ConnectorUtilities.ComputePoint(to.Bounds, to.IncomingDocking, to.IncomingRelativePosition);
            var pos = ConnectorUtilities.ComputeRelativePositionOfPoints(startPoint, endPoint);

            LineType lineType = (LineType)((int)from.OutgoingDocking << 8 | (int)to.IncomingDocking);

            if (AllowedRelativePositions.Table.TryGetValue(lineType, out SortedSet<RelativePosition> possibleConfigurations))
            {
                if (!possibleConfigurations.Contains(pos))
                {
                    if (_model.CanMoveStart)
                    {
                        from.OutgoingDocking = ConnectorDocking.Undefined;
                    }
                    if (_model.CanMoveEnd)
                    {
                        to.IncomingDocking = ConnectorDocking.Undefined;
                    }
                }
            }
            else
            {
                if (_model.CanMoveStart)
                {
                    from.OutgoingDocking = ConnectorDocking.Undefined;
                }
                if (_model.CanMoveEnd)
                {
                    to.IncomingDocking = ConnectorDocking.Undefined;
                }
            }
        }

        private void RegisterHandler(IEditOperation handler)
        {
            _currentOperationHandler = handler;
        }

        void UpdateLabel(object oldValue, object newValue)
        {
            
            if( oldValue is ConnectorLabelModel oldLabel )
            {
                _parent.SketchItems.Remove(oldLabel);
                    
            }
            if( newValue is ConnectorLabelModel newLabel)
            {
                _parent.SketchItems.Add(newLabel);
            }
            
        }

        void NotifySelectionChanged()
        {
            if (IsSelected)
            {
                _lineWidth = 3;
                _lineBrush = _selectedLineBrush;
                Fill = Brushes.Blue;
                
            }
            else
            {
                _lineWidth = 1;
                _lineBrush = Brushes.Black;
                Fill = Brushes.Black;
            }
            if (_model.FillEndings == false)
            {
                Fill = Brushes.White;
            }
            StrokeThickness = _lineWidth;
            Stroke = _lineBrush;
            
            _linePen.Brush = Stroke;
            _linePen.Thickness = StrokeThickness;

            SelectionChanged?.Invoke(this, IsSelected);
          
            InvalidateVisual();
            _parent.ShowIntersections();

        }

        void NotifyIsMarkedChanged()
        {
            IsMarkedChanged?.Invoke(this, IsMarked);
        }

        void AddWaypoint()
        {
            Point pos = PlacementHelper.RoundToGrid(_lastContextMenuPosition);

            var marker = new Waypoint( pos);
            _model.AddWaypoint(marker);
        }

        private void OnWaypoint_DragDelta(object sender, DragDeltaEventArgs e)
        {
            
        }

        private void MyGeometry_Changed(object sender, EventArgs args)
        {
            InvalidateVisual();
        }

        private static void OnSelectedChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {

            ConnectorUI connector = source as ConnectorUI;
            if (e.NewValue != e.OldValue)
            {
                bool isSelected = (bool)e.NewValue;
                var zIndex = 0;
                if( isSelected)
                {
                    zIndex = 10;
                }
                Canvas.SetZIndex(connector.Shape, zIndex);
                
                connector.NotifySelectionChanged();
                
            }
        }

        private static void OnIsMarkedChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {

            ConnectorUI connector = source as ConnectorUI;
            if (e.NewValue != e.OldValue)
            {
                connector.NotifyIsMarkedChanged();
            }
        }

        private static void OnConnectorLineStyleChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {

        }

        private static void OnConnectorLineWidthChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {

        }

        private static void OnContextMenuDeclarationChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {
            
            if( source is ConnectorUI me)
            {
                if( e.NewValue is IList<ICommandDescriptor>  commands)
                {
                    if (me._model.AllowWaypoints)
                    {
                        commands.Add(CreateAddWaypointMenuItem(me));
                    }
                    me.ContextMenu = OutlineHelper.InitContextMenu(commands);
                }
            }
        }

        private static void OnConnectorStartLabelChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            if( source is ConnectorUI me)
            {
                me.UpdateLabel(e.OldValue, e.NewValue);
            }   
        }

        private static void OnConnectorEndLabelChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            if( source is ConnectorUI me)
            {
                me.UpdateLabel(e.OldValue, e.NewValue);
            }
        }

        private static void OnFromChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            if( source is ConnectorUI me)
            {
                if (e.OldValue is ConnectableBase oldFrom)
                {
                    oldFrom.ShapeChanged -= me.From_ShapeChanged;
                }
                
                
                if ( e.NewValue is ConnectableBase newFrom )
                {
                    var model = (ConnectorModel)me.Model;
                    newFrom.ShapeChanged += me.From_ShapeChanged;
                    if (!model.IsRewireing)
                    {
                        me.CheckLineStart();
                        me.UpdateGeometry();
                    }
                }
                else
                {
                    me._parent.BeginEdit(new RewireConnectorOperation(me._parent, me._model, me._model.ConnectorStart));
                }

            }
        }

        private static void OnToChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            
            if (source is ConnectorUI me)
            {
                if( e.OldValue is ConnectableBase oldTo)
                {
                    oldTo.ShapeChanged -= me.To_ShapeChanged;
                }
                
                if( e.NewValue is ConnectableBase newTo)
                {
                    newTo.ShapeChanged += me.To_ShapeChanged;
                    me.CheckLineEnding();
                    me.UpdateGeometry();
                }
                else
                {
                    me._parent.BeginEdit(new RewireConnectorOperation(me._parent, me._model, me._model.ConnectorEnd));
                }
                
            }
        }

        private static void OnWaypointsChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            if(source is ConnectorUI me)
            {
                if( e.OldValue != e.NewValue)
                {
                    if (me._addornerAdded)
                    {
                        me._myAdorner.InvalidateVisual();
                    }
                }
            }
        }

        

        private static void OnGeometryChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {
            
            if (source is ConnectorUI me  && e.OldValue != e.NewValue)
            {
                if (e.OldValue is Geometry oldGeometry)
                {
                    oldGeometry.Changed -= me.MyGeometry_Changed;
                }
                me._myGeometry = e.NewValue as Geometry;
                me.RefModel?.UpdateGeometry();
                me._myGeometry.Changed += me.MyGeometry_Changed;
                me._myAdorner?.InvalidateVisual();
            }
            
          
        }


        private static ICommandDescriptor CreateAddWaypointMenuItem(ConnectorUI ui)
        {
            var descriptor = new CommandDescriptor()
            {
                Name = "Add Waypoint",
                Command = new DelegateCommand(ui.AddWaypoint)
            };
            return descriptor;
        }

        

        public void Dispose(){}
    }
}
