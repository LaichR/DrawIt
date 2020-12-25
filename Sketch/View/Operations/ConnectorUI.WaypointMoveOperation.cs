using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sketch.Interface;
using System.Windows.Input;
using System.Windows.Media;

namespace Sketch.View
{
    public partial class ConnectorUI
    {
        internal class WaypointMoveOperation : IEditOperation
        {
            readonly IWaypoint _wp;
            readonly ConnectorUI _ui;
            bool _done;
            Point _startPoint;
            public WaypointMoveOperation(ConnectorUI ui, Point startPoint, IWaypoint wp)
            {
                _ui = ui;
                _wp = wp;
                _startPoint = startPoint;
                _ui.TriggerSnapshot();

                _ui.MouseMove += Ui_MouseMove;
                _ui.MouseUp += Ui_MouseUp; 
                _ui.CaptureMouse();


            }

            private void Ui_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
            {
                var commit = !Keyboard.IsKeyDown(Key.Escape);
                StopOperation(commit);
            }

            private void Ui_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
            {
            
                Point p = e.GetPosition(_ui._parent.Canvas);

                p.X = Math.Round(p.X / SketchPad.GridSize) * SketchPad.GridSize;
                p.Y = Math.Round(p.Y / SketchPad.GridSize) * SketchPad.GridSize;

                var v = Point.Subtract(p, _startPoint);
                _startPoint = p;
                _wp.Move(new TranslateTransform(v.X, v.Y));
                e.Handled = true;
            }

            public void OnKeyDown(System.Windows.Input.KeyEventArgs e)
            {
                if (e.Key == System.Windows.Input.Key.Escape)
                {
                    _ui.DropSnapshot();
                    StopOperation(false);
                }
            }

            public void StopOperation(bool commit)
            {
                if (!_done)
                {
                    _done = true;

                    _ui.MouseMove -= Ui_MouseMove;
                    _ui.MouseUp -= Ui_MouseUp;
                    _ui.ReleaseMouseCapture();
                    if (!commit)
                    {
                        _ui.DropSnapshot();
                    }
                }
                _ui.UpdateGeometry();
            }
        }
    }
}
