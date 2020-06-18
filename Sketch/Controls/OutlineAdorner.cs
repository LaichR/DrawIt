using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Controls;
using Sketch.Models;
using Sketch.Types;

namespace Sketch.Controls
{
    class OutlineAdorner: Adorner
    {
        static readonly Brush _selectedOutlineBrush = new SolidColorBrush(Colors.Blue) { Opacity = 0.5 };
        static readonly DashStyle _activeDashStile = new DashStyle(new double[] { 2.0, 2.5 }, 0.0);
        static readonly double _activeStroke = 1.0;
        static readonly double _defaultStroke = 3.0;
        static readonly Pen _hitTestPen = new Pen(Brushes.White, 20.0);

        ConnectableBase _realBody;
        RectangleGeometry _shadowGeometry;
        readonly List<Rect> _sensitiveBorder = new List<Rect>();
                


        Brush _myBrush;
        Pen _myPen;
        bool _moving = false;
        bool _isActive = false;
        bool _allowResize;
        System.Windows.Point _start = new Point();
        OutlineUI _adorned;
        SketchPad _parent;
        double _strokeThickness = 3.0;
        DashStyle _defaultDashstile;
        
        public OutlineAdorner(OutlineUI adorned, SketchPad parent )
            :base(adorned)
        {
            _adorned = adorned;
            _parent = parent;
            _realBody = _adorned.Model as ConnectableBase;
            _shadowGeometry = _realBody.Outline;
            _myBrush = _selectedOutlineBrush;
            ComputeSensitiveBorder();
            _myPen = new Pen(_myBrush, _defaultStroke);
            _defaultDashstile = _myPen.DashStyle;
            this.Visibility = System.Windows.Visibility.Visible;
        }

        internal void SetActive(bool active)
        {
            _isActive = active;
            if (active)
            {
                _myPen.Thickness = _activeStroke;
                _myPen.DashStyle = _activeDashStile;

            }
            else
            {
                _myPen.DashStyle = _defaultDashstile;
                _myPen.Thickness = _defaultStroke;
                InvalidateVisual(); // seems to be required in order to display propertly!
            }
            
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawGeometry(null, _myPen, _shadowGeometry);
        }

        public void Transform( Transform transform)
        {
            if (transform != null)
            {
                TransformGroup tg = new TransformGroup();

                tg.Children.Add(_realBody.Rotation);
                tg.Children.Add(transform);
                _shadowGeometry.Transform = tg;
                _parent.BringIntoView(_shadowGeometry.Bounds);
                InvalidateVisual();
            }
               
        }

        internal RelativePosition HitShadowBorder(Point p)
        {
            if (!_allowResize) return RelativePosition.Undefined;
            int direction = 0;
            for (int i = 0; i < _sensitiveBorder.Count; ++i)
            {
                if (_sensitiveBorder[i].Contains(p))
                {
                    direction |= (1 << i);
                }
            }
            return (RelativePosition)direction;
        }

        internal void SetEnableResizeOperation( bool set)
        {
            _allowResize = set;
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            Point p = e.GetPosition(this._parent);
            p.X = Math.Round(p.X / _parent.Grid) * _parent.Grid;
            p.Y = Math.Round(p.Y / _parent.Grid) * _parent.Grid;

            if( _adorned.LabelArea.Contains(p) && _adorned.Model.AllowEdit )
            {
                Mouse.OverrideCursor = Cursors.Hand;
            }
            else
            {
                if (_allowResize)
                {
                    var dir = HitShadowBorder(p);
                    switch (dir)
                    {
                        case RelativePosition.N:
                        case RelativePosition.S:
                            Mouse.OverrideCursor = Cursors.SizeNS;
                            break;
                        case RelativePosition.E:
                        case RelativePosition.W:
                            Mouse.OverrideCursor = Cursors.SizeWE;
                            break;
                        case RelativePosition.NW:
                        case RelativePosition.SE:
                            Mouse.OverrideCursor = Cursors.SizeNWSE;
                            break;
                        case RelativePosition.SW:
                        case RelativePosition.NE:
                            Mouse.OverrideCursor = Cursors.SizeNESW;
                            break;
                        default:
                            if (!_isActive)
                            {
                                Mouse.OverrideCursor = null;
                            }
                            break;
                    }
                }
            }
            e.Handled = true;
        }


        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (!_isActive)
            {
                Mouse.OverrideCursor = null;
            }
            e.Handled = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            _parent.HandleKeyDown( e);
        }

        internal void UpdateGeometry()
        {
            _shadowGeometry = _realBody.Outline;
            ComputeSensitiveBorder();
            InvalidateVisual();
        }

        void ComputeSensitiveBorder()
        {
            double halfWidth =  _hitTestPen.Thickness/2;
            Rect r = _shadowGeometry.Bounds;
            _sensitiveBorder.Clear();
            var leftTop = new Point(r.Left - halfWidth, r.Top - halfWidth);
            var rightTopTop = new Point(r.Right - halfWidth, r.Top - halfWidth);
            var rightTopBottom = new Point(r.Right + halfWidth, r.Top + halfWidth);
            var leftBootomTop = new Point(r.Left - halfWidth, r.Bottom - halfWidth);
            var leftBootomBottom = new Point(r.Left + halfWidth, r.Bottom + halfWidth);
            var rightBottom = new Point(r.Right + halfWidth, r.Bottom + halfWidth);
            _sensitiveBorder.Add(new Rect(leftTop, rightTopBottom));
            _sensitiveBorder.Add(new Rect(rightTopTop, rightBottom));
            _sensitiveBorder.Add(new Rect(leftBootomTop, rightBottom));
            _sensitiveBorder.Add(new Rect(leftTop, leftBootomBottom));
        }
    }
}
