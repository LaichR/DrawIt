using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using Sketch.Models;
using System.Windows.Input;
using Sketch.Interface;

namespace Sketch.Controls
{
    class ConnectorAdorner: Adorner
    {
        readonly ConnectorModel _model;

        readonly ISketchItemDisplay _parent;
        double _lineWidht = 3;
        Brush _myLineBrush;
        Brush _myFillBrush;
        Pen _myPen;
        
        public ConnectorAdorner(ISketchItemDisplay parent, ConnectorUI ui)
            :base(ui)
        {
            _parent = parent;
            _model = ((ConnectorUI)this.AdornedElement).Model as ConnectorModel;
            this.Visibility = System.Windows.Visibility.Visible;
            this._model.PropertyChanged += OnModelPropertyChanged;
            
            IsHitTestVisible = true;
            ui.InvalidateVisual();
        }


        public void Refresh( bool isSelected )
        {
            if (isSelected )
            {               
                _myLineBrush = Brushes.Blue;
                _lineWidht = 3;
            }
            else
            {
                _myLineBrush = Brushes.Black;
                _lineWidht = 1;
                HitEnd = false;
                HitStart = false;
            }

            _myFillBrush = _myLineBrush;

            _myPen = new Pen(_myLineBrush, _lineWidht);
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (HitStart)
            {
                drawingContext.DrawRectangle(_myFillBrush, _myPen, _model.HotSpotStart);
            }
            if (HitEnd)
            {
                drawingContext.DrawRectangle(_myFillBrush, _myPen, _model.HotSpotEnd);
            }   
        }

        public bool HitStart
        {
            get
            {
                return _model.ConnectorStartSelected;
            }
            private set
            {
                _model.ConnectorStartSelected = value;
            }
        }

        public bool HitEnd
        {
            get
            {
                return _model.ConnectorEndSelected;
            }
            private set
            {
                _model.ConnectorEndSelected = value;
            }
        }


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var p = MousePositionArea( e.GetPosition(_parent.Canvas) );

            var toggleSelection = Keyboard.Modifiers != ModifierKeys.Control;
            
            if( _model.HotSpotEnd.IntersectsWith(p) )
            {
                HitEnd = true;
                if(toggleSelection)
                {
                    HitStart = false;
                }
            }
            else if( _model.HotSpotStart.IntersectsWith(p))
            {
                HitStart = true;
                if( toggleSelection)
                {
                    HitEnd = false;
                }
            }
            
        }


        void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if( e.PropertyName == "IsSelected" )
            {
                Refresh(_model.IsSelected);
                this.IsHitTestVisible = _model.IsSelected;
                
                
            }
        }


        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            var p = MousePositionArea(hitTestParameters.HitPoint);
            var hitStart = _model.HotSpotStart.IntersectsWith(p);
            var hitEnd = _model.HotSpotEnd.IntersectsWith(p);
            if( hitStart || hitEnd )
            {                
                var result = new PointHitTestResult(this, hitTestParameters.HitPoint);
                InvalidateVisual();
                return result;
            }
            return null;
        }

        Rect MousePositionArea(Point p)
        {
            return new Rect(p.X - 1, p.Y - 1, 2, 2);
        }

    }
}
