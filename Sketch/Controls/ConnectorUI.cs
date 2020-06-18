
using System.Windows.Controls;
using UI.Utilities.Interfaces;

using Sketch.Models;
using Sketch.Types;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Sketch.Interface;

namespace Sketch.Controls
{
    public partial class ConnectorUI: Shape, IGadgetUI
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

        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(ConnectableBase), typeof(ConnectorUI),
            new PropertyMetadata(OnFromChanged));

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(ConnectableBase), typeof(ConnectorUI),
            new PropertyMetadata(OnToChanged));

        public static readonly DependencyProperty ContextMenuDeclarationProperty =
            DependencyProperty.Register("ContextMenuDeclaration", typeof(IList<ICommandDescriptor>), typeof(ConnectorUI),
            new PropertyMetadata(OnContextMenuDeclarationChanged));



        ConnectorModel _model;
        SketchPad _parent;
        Geometry _myGeometry = null;
        IEditOperation _currentOperationHandler;
        int _lineWidth;
        Brush _lineBrush;
        static Brush _selectedLineBrush = new SolidColorBrush(Colors.Blue) { Opacity = 0.5 };
        static Pen _hitTestPen = new Pen() { Thickness = 20 };

        Adorner _myAdorner;
        bool _addornerAdded = false;


        public ConnectorUI(SketchPad parent, ConnectorModel model)
        {
            _model = model;
            _parent = parent;
            this.DataContext = _model;

            _myAdorner = new ConnectorAdorner(_parent, this);

            var isSelectedBinding = new Binding("IsSelected");
            isSelectedBinding.Mode = BindingMode.TwoWay;
            this.SetBinding(IsSelectedPropery, isSelectedBinding );

            var startLabelBinding = new Binding("ConnectorStartLabel");
            startLabelBinding.Mode = BindingMode.OneWay;
            this.SetBinding(ConnectorStartLabelProperty, startLabelBinding);

            var contextMenuBinding = new Binding("ContextMenuDeclaration");
            contextMenuBinding.Mode = BindingMode.OneWay;
            this.SetBinding(ContextMenuDeclarationProperty, contextMenuBinding);

            var fromBinding = new Binding("From");
            fromBinding.Mode = BindingMode.OneWay;
            this.SetBinding(FromProperty, fromBinding);

            var toBinding = new Binding("To");
            toBinding.Mode = BindingMode.OneWay;
            this.SetBinding(ToProperty, toBinding);

            SetBinding(StrokeDashArrayProperty,
                new Binding("StrokeDashArray"));

            _model.From.ShapeChanged += From_ShapeChanged;
            _model.To.ShapeChanged += To_ShapeChanged;
         
            this.Visibility = System.Windows.Visibility.Visible;
            this.Stroke = Brushes.White;
            

            this.StrokeThickness = 1;// ComputeConnectorLine.LineWidth;
            _myGeometry = _model.Geometry;

            AddAdorner();
            _myAdorner.MouseLeftButtonDown += Adorner_MouseLeftButtonDown;
            this.SetBinding(GeometryProperty, "Geometry");
            
        }

        private void Adorner_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.OnMouseLeftButtonDown(e);
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

        public Shape Shape
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

        public ModelBase Model
        {
            get { return _model; }
        }

        public ISketchItemModel RefModel
        {
            get { return _model.RefModel; }
        }
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
        
            _parent.Focus();
            
            if( _currentOperationHandler != null )
            {
                _currentOperationHandler.StopOperation(false);
                _currentOperationHandler = null;
                return;
            }
            
            e.Handled = true;
            
            var p = e.GetPosition(_parent);
            _currentOperationHandler = new MoveConnectorOperation(this, p );

            if( IsSelected)
            {
                NotifySelectionChanged();
                return;
            }
            IsSelected = true;
            
        }


        void OnShadowMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            //e.Handled = true;
        }

        void OnShadowMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnMouseLeftButtonDown(e);
        }

        void AddAdorner()
        {
            var adornderlayer = AdornerLayer.GetAdornerLayer(this);
            if (adornderlayer == null) return;
            
            adornderlayer.Add(_myAdorner);
            _addornerAdded = true;
        }

        private void  UpdateGeometry()
        {
            _model.TriggerGeometryUpdate();

            InvalidateVisual();
            var intersections = _parent.Intersections;
            if (intersections != null)
            {
                intersections.InvalidateVisual();
            }
            
            
            if (!_addornerAdded)
            {
                AddAdorner();
            }
            _myAdorner.InvalidateVisual();
        }

        void To_ShapeChanged(object sender, OutlineChangedEventArgs e)
        {
            // make a quick plausibility test in order to avoid overlapping of connector and shape!
            CheckLineConfigurations();
            UpdateGeometry();
            
        }

        void From_ShapeChanged(object sender, OutlineChangedEventArgs e)
        {
            // make a quick plausibility test in order to avoid overlapping of connector and shape!
            CheckLineConfigurations();
            UpdateGeometry();
        }

        void CheckLineConfigurations()
        {
            if (_model.From == null || _model.To == null) return;

            

            // nothing to do for self transistions!
            if (_model.From == _model.To) return;

            var startPoint = ConnectorUtilities.ComputePoint(_model.From.Bounds, _model.StartPointDocking, _model.StartPointRelativePosition);
            var endPoint = ConnectorUtilities.ComputePoint(_model.To.Bounds, _model.EndPointDocking, _model.EndPointRelativePosition);
            var pos = ConnectorUtilities.ComputeRelativePositionOfPoints(startPoint, endPoint);

            LineType lineType = (LineType)((int)_model.StartPointDocking << 8 | (int)_model.EndPointDocking);
            SortedSet<RelativePosition> possibleConfigurations = null;

            if (AllowedRelativePositions.Table.TryGetValue(lineType, out possibleConfigurations))
            {
                if (!possibleConfigurations.Contains(pos))
                {
                    _model.StartPointDocking = ConnectorDocking.Undefined;
                    _model.EndPointDocking = ConnectorDocking.Undefined;
                }
                
            }
            else
            {
                _model.StartPointDocking = ConnectorDocking.Undefined;
                _model.EndPointDocking = ConnectorDocking.Undefined;
            }
        }

        private void RegisterHandler(IEditOperation handler)
        {
            _currentOperationHandler = handler;
        }

        void UpdateLabel(object oldValue, object newValue)
        {
            {
                var oldLabel = oldValue as ConnectorLabelModel;
                if (oldLabel != null)
                {
                    _parent.RemoveVisualChild(oldLabel);
                    
                }
                var newLabel = newValue as ConnectorLabelModel;
                if( newValue != null)
                {
                    _parent.AddVisualChild(newLabel);
                }
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
            StrokeThickness = _lineWidth;
            Stroke = _lineBrush;

            if (SelectionChanged != null)
            {
                SelectionChanged(this, IsSelected);
            }

            var intersections = _parent.Intersections;
            if (intersections != null)
            {
                intersections.InvalidateVisual();
            }

        }

        void NotifyIsMarkedChanged()
        {
            if (IsMarkedChanged != null)
            {
                IsMarkedChanged(this, IsSelected);
            }
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
            var me = source as ConnectorUI;
            if( me != null)
            {
                var commands = e.NewValue as IList<ICommandDescriptor>;
                if (commands != null)
                {
                    me.ContextMenu = OutlineHelper.InitContextMenu(commands);
                }
            }
        }

        private static void OnConnectorStartLabelChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {

            var me = source as ConnectorUI;
            if (me != null)
            {
                me.UpdateLabel(e.OldValue, e.NewValue);
            }
            
        }

        private static void OnConnectorEndDecorationChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {

        }

        private static void OnFromChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            var me = source as ConnectorUI;
            if( me != null)
            {
                var oldFrom = e.OldValue as ConnectableBase;
                if (oldFrom != null)
                {
                    oldFrom.ShapeChanged -= me.From_ShapeChanged;
                }
                var newFrom = e.NewValue as ConnectableBase;
                var model = (ConnectorModel)me.Model;
                if ( newFrom != null )
                {
                    newFrom.ShapeChanged += me.From_ShapeChanged;
                    if (!model.IsRewireing)
                    {
                        me.CheckLineConfigurations();
                        me.UpdateGeometry();
                    }
                }
                else
                {
                    me._parent.RegisterHandler(new SketchPad.RewireConnectorOperation(me._parent, me._model, me._model.ConnectorStart));
                }

            }
        }

        private static void OnToChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            var me = source as ConnectorUI;
            if (me != null)
            {
                var oldTo = e.OldValue as ConnectableBase;
                if( oldTo != null)
                {
                    oldTo.ShapeChanged -= me.To_ShapeChanged;
                }
                var newTo = e.NewValue as ConnectableBase;
                if( newTo != null)
                {
                    newTo.ShapeChanged += me.To_ShapeChanged;
                    me.CheckLineConfigurations();
                    me.UpdateGeometry();
                }
                else
                {
                    me._parent.RegisterHandler(new SketchPad.RewireConnectorOperation(me._parent, me._model, me._model.ConnectorEnd));
                }

                
            }
        }

        private static void OnGeometryChanged(DependencyObject source,
             DependencyPropertyChangedEventArgs e)
        {

            var me = source as ConnectorUI;
            if (me != null && e.OldValue != e.NewValue)
            {
                me._myGeometry = e.NewValue as Geometry;
            }
        }
    }
}
