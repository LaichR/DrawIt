using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Sketch.Helper;


namespace Sketch.Controls
{
    public partial class OutlineUI 
    {
        class MoveOperation : IEditOperation
        {
            Point _start;
            readonly OutlineUI _ui;
            bool _done = false;
            Transform _moveTransform;

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
                
            }

            Transform ComputeMoveTransformation(Point p)
            {
                var v = Point.Subtract(p, _start);
                v.X = Math.Round(Math.Round(v.X / SketchPad.GridSize) * SketchPad.GridSize,2);
                v.Y = Math.Round(Math.Round(v.Y / SketchPad.GridSize) * SketchPad.GridSize,2);
                var translation = new TranslateTransform(v.X, v.Y);
                return translation;
            }

            void HandleMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
            {
                Point p = e.GetPosition(this._ui._parent.Canvas);
                _moveTransform = ComputeMoveTransformation(p);
                _ui._adorner.Transform(_moveTransform);
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
