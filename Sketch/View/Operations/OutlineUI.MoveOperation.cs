using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using Sketch.Helper;


namespace Sketch.View
{
    public partial class OutlineUI 
    {
        class MoveOperation : IEditOperation
        {
            Point _start;
            readonly OutlineUI _ui;
            bool _done = false;
            TranslateTransform _moveTransform;
            RotateTransform _rotateTransform;
            RotateTransform _rotateTransform1;

            public MoveOperation(OutlineUI parent_, Point p)
            {
                _start = p;
                _ui = parent_;
                
                //_originalRect = _ui.Bounds;
                _ui.MouseUp += HandleMouseUp;
                _ui.MouseMove += HandleMouseMove;
                _ui._adorner.SetActive(true);
                _ui.CaptureMouse();
                _ui.TriggerSnapshot();
                _rotateTransform = new RotateTransform(90, _ui.Bounds.Width / 2, _ui.Bounds.Height/2 );
                _rotateTransform1 = new RotateTransform(-90, _ui.Bounds.Width / 2, _ui.Bounds.Height / 2);

            }

            TranslateTransform ComputeMoveTransformation(Point p)
            {
                var v = Point.Subtract(p, _start);
                v = PlacementHelper.RoundToGrid(v);
                var translation = new TranslateTransform(v.X, v.Y);
                return translation;
            }

            void HandleMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
            {
                Point p = e.GetPosition(this._ui._parent.Canvas);
                _moveTransform = ComputeMoveTransformation(p);
                var v = _ui._model.Rotation.Transform(p) - _ui._model.Rotation.Transform(_start);
               
                var translateTransform1 = new TranslateTransform(v.X, v.Y);
                //Canvas.SetLeft( _ui._adorner, Canvas.GetLeft(_ui._adorner) - t.X);
                //Canvas.SetTop(_ui._adorner, Canvas.GetTop(_ui._adorner) - t.Y);
                //var groupTransform = new TransformGroup();
                
                //groupTransform.Children.Add(_rotateTransform1);
                //groupTransform.Children.Add(_moveTransform);
                //groupTransform.Children.Add(_rotateTransform);
                _ui._adorner.Transform(translateTransform1);
                
                e.Handled = true;
            }

            void HandleMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
            {
                StopOperation(true);
                e.Handled = true;
               
            }

            public void StopOperation(bool commit)
            {
                if (!_done)
                {
                    _done = true;
                    _ui.ReleaseMouseCapture();
                    _ui.MouseUp -= HandleMouseUp;
                    _ui.MouseMove -= HandleMouseMove;
                    _ui.ReleaseMouseCapture();
                    _ui._adorner.SetActive(false);
                    _ui.RegisterHandler(null);
                    if (commit && _moveTransform != null)
                    {
                        _ui.Model.Move(_moveTransform);
                        _ui._adorner.UpdateGeometry();
                    }
                    else
                    {
                        _ui.DropSnapshot();
                    }
                }
                
            }
        }
    }
}
