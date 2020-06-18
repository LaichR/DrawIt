using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Sketch.Types;

namespace Sketch.Controls
{
    public partial class OutlineUI
    {
        class ChangeSizeOperation : IEditOperation
        {
            OutlineUI _ui;
            Point _start;
            Pen _pen = new Pen(Brushes.Black, 30);
            RelativePosition _direction;
            Rect _origialRect;
            Transform _resizeTransform;
            Vector _horizontalLine = new Vector(100, 0);
            bool _done = false;

            public ChangeSizeOperation( OutlineUI parent_, Point p )
            {
                _start = p;
                _ui = parent_;
                _origialRect = _ui.Bounds;
                List<LineGeometry> lgs = new List<LineGeometry>
                {
                    new LineGeometry( _origialRect.TopLeft, _origialRect.TopRight ),
                    new LineGeometry( _origialRect.TopRight, _origialRect.BottomRight),
                    new LineGeometry( _origialRect.BottomRight, _origialRect.BottomLeft),
                    new LineGeometry( _origialRect.BottomLeft, _origialRect.TopLeft)
                };

                foreach( var l in lgs)
                {
                    l.Transform = _ui.DefiningGeometry.Transform;
                }

                int direction = 0;
                for (int i = 0; i < lgs.Count; ++i)
                {
                    if (lgs[i].StrokeContains(_pen, _start))
                    {
                        direction |= (1 << i);
                    }
                }

                _ui.TriggerSnapshot();
                _ui._adorner.SetActive(true);
                _direction = (RelativePosition)direction;// _ui._myShadow.HitShadowBorder(_start);
                _ui.MouseUp += HandleMouseUp;
                _ui.MouseMove += HandleMouseMove;
                _ui.CaptureMouse();
                
                if( _direction == RelativePosition.Undefined)
                {
                    StopOperation(false);
                }
            }

            void HandleMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
            {
                Point p = e.GetPosition(this._ui._parent);
                this._resizeTransform = ComputeResizeTransformation(p);
                _ui._adorner.Transform(_resizeTransform);
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
                    if (commit && _resizeTransform != null)
                    {
                        _ui.Model.Move(_resizeTransform);
                        _ui._adorner.UpdateGeometry();
                        _ui._parent.EditMode = EditMode.Select;
                        
                    }
                    else
                    {
                        _ui.DropSnapshot();
                    }
                }
                
            }

           

            public void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e){}

 




            Transform ComputeResizeTransformation(Point p)
            {
                p.X = Math.Round(p.X / this._ui._parent.Grid) * this._ui._parent.Grid;
                p.Y = Math.Round(p.Y / this._ui._parent.Grid) * this._ui._parent.Grid;

                var rect = _origialRect;
                var rot = _ui._geometry.Transform as RotateTransform;
                var angle = 0.0;
                if( rot != null)
                {
                    angle = rot.Angle;
                }
                switch( _direction)
                {
                    case RelativePosition.N:
                        {
                            var delta = _start - p;
                            //var len = delta.Length;
                            //angle = Vector.AngleBetween(delta, _horizontalLine)-angle;
                            //var deltaY = Math.Sin(angle) * len;
                            var scaleY = (delta.Y + _origialRect.Height) / _origialRect.Height;
                            //var scaleY = ( deltaY + _origialRect.Height) / _origialRect.Height;
                            var sf = new ScaleTransform(1, scaleY);
                            rect.Transform(sf.Value);
                            var transY = _origialRect.Top -rect.Top - delta.Y;
                            //var transY = _origialRect.Top - rect.Top - deltaY; 
                            var tg = new TransformGroup();
                            tg.Children.Add(sf);
                            tg.Children.Add(new TranslateTransform(0, transY));
                            
                            return tg;
                        }
                    case RelativePosition.NE:
                        {
                            var delta = _start - p;
                            var scaleY = (delta.Y + _origialRect.Height) / _origialRect.Height;
                            var scaleX = (-delta.X + _origialRect.Width) / _origialRect.Width;
                            var sf = new ScaleTransform(scaleX, scaleY);
                            rect.Transform(sf.Value);
                            var trans = _origialRect.TopLeft - rect.TopLeft;
                            var tg = new TransformGroup();
                            tg.Children.Add(sf);
                            tg.Children.Add(new TranslateTransform(trans.X, trans.Y - delta.Y));
                            return tg;
                        }
                    case RelativePosition.E:
                        {
                            var delta = p-_start;
                            
                            var scaleX = (delta.X + _origialRect.Width) / _origialRect.Width;
                            var sf = new ScaleTransform(scaleX, 1);
                            rect.Transform(sf.Value);
                            var trans = _origialRect.TopLeft - rect.TopLeft;
                            var tg = new TransformGroup();
                            tg.Children.Add(sf);
                            tg.Children.Add(new TranslateTransform(trans.X, 0));
                            return tg;

                        }
                    case RelativePosition.SE:
                        {
                            var delta = p -_start;
                            var scaleY = (delta.Y + _origialRect.Height) / _origialRect.Height;
                            var scaleX = (delta.X + _origialRect.Width) / _origialRect.Width;
                            var sf = new ScaleTransform(scaleX, scaleY);
                            rect.Transform(sf.Value);
                            var trans = _origialRect.TopLeft - rect.TopLeft;
                            var tg = new TransformGroup();
                            tg.Children.Add(sf);
                            tg.Children.Add(new TranslateTransform(trans.X, trans.Y));
                            return tg;
                        }
                    case RelativePosition.S:
                        {
                            var delta = p - _start;
                            var scaleY = (delta.Y + _origialRect.Height) / _origialRect.Height;
                            var sf = new ScaleTransform(1, scaleY);
                            rect.Transform(sf.Value);
                            var trans = _origialRect.TopLeft - rect.TopLeft;
                            var tg = new TransformGroup();
                            tg.Children.Add(sf);
                            tg.Children.Add(new TranslateTransform(0, trans.Y));
                            return tg;
                        }
                    case RelativePosition.SW:
                        {
                            var delta = p - _start;
                            var scaleY = (delta.Y + _origialRect.Height) / _origialRect.Height;
                            var scaleX = (-delta.X + _origialRect.Width) / _origialRect.Width;
                            var sf = new ScaleTransform(scaleX, scaleY);
                            rect.Transform(sf.Value);
                            var trans = _origialRect.TopLeft - rect.TopLeft;
                            var tg = new TransformGroup();
                            tg.Children.Add(sf);
                            tg.Children.Add(new TranslateTransform(trans.X + delta.X, trans.Y));
                            return tg;
                        }
                    case RelativePosition.W:
                        {
                            var delta = p - _start;
                            var scaleX = (-delta.X + _origialRect.Width) / _origialRect.Width;
                            var sf = new ScaleTransform(scaleX, 1);
                            rect.Transform(sf.Value);
                            var trans = _origialRect.TopLeft - rect.TopLeft;
                            var tg = new TransformGroup();
                            tg.Children.Add(sf);
                            tg.Children.Add(new TranslateTransform(trans.X + delta.X, 0));
                            return tg;
                        }
                    case RelativePosition.NW:
                        {
                            var delta = p - _start;
                            var scaleY = (-delta.Y + _origialRect.Height) / _origialRect.Height;
                            var scaleX = (-delta.X + _origialRect.Width) / _origialRect.Width;
                            var sf = new ScaleTransform(scaleX, scaleY);
                            rect.Transform(sf.Value);
                            var trans = _origialRect.TopLeft - rect.TopLeft;
                            var tg = new TransformGroup();
                            tg.Children.Add(sf);
                            tg.Children.Add(new TranslateTransform(trans.X + delta.X, trans.Y + delta.Y));
                            return tg;
                        }
                    default:
                        return null;
                }
                return null;
            }
        }
    }
}
