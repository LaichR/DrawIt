using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Sketch.Models;
using Sketch.Interface;


namespace Sketch.View
{
    class DecoratorAdorner: Adorner
    {
        readonly ConnectableBase _model;
        readonly ISketchItemDisplay _parent;
        public DecoratorAdorner(UIElement uiElement, ISketchItemDisplay parent) :
            base(uiElement)
        {
            _parent = parent;
            if( uiElement is OutlineUI outline &&
                outline.Model is ConnectableBase connectable)
            {
                _model = connectable;
            }
            //IsHitTestVisible = false;
            
        }

        

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
    
            foreach( var d in _model.Decorators)
            {
                drawingContext.DrawGeometry(d.Fill, d.Pen, d.Geometry);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                e.Source = AdornedElement;
                e.Handled = false;
                AdornedElement.RaiseEvent(e);
            }));
            
        }
    }
}
