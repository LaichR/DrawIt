using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using Sketch.Interface;
using Sketch.Models;


namespace Sketch.Controls
{
    internal class SelectUisOperation : IEditOperation
    {

        readonly ISketchItemDisplay _pad;
        Point _startPoint;
        Rectangle _selectionAreaVisualizer;
        //bool _selecting = false;

        public SelectUisOperation(ISketchItemDisplay pad)
        {
            _pad = pad;
            _pad.Canvas.MouseLeftButtonDown += MouseLeftButtonDown;
            _pad.Canvas.KeyDown += KeyDown;
        }

        void KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            //if( _pad._S)
            switch (e.Key)
            {
                case Key.Left:
                    MoveMarked(-1 * SketchPad.GridSize, 0);
                    break;
                case Key.Right:
                    MoveMarked(1 * SketchPad.GridSize, 0);
                    break;
                case Key.Up:
                    MoveMarked(0, -1 * SketchPad.GridSize);
                    break;
                case Key.Down:
                    MoveMarked(0, 1 * SketchPad.GridSize);
                    break;
                default:
                    e.Handled = false;
                    break;
            }
        }

        void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            _pad.Canvas.MouseLeftButtonUp -= MouseLeftButtonUp;
            _pad.Canvas.MouseMove -= MouseMove;
            var ok = e.RightButton == MouseButtonState.Released;
            if (ok)
            {
                _pad.ClearSelection();
                var left = Canvas.GetLeft(_selectionAreaVisualizer);
                var top = Canvas.GetTop(_selectionAreaVisualizer);

                var selectionArea = new Rect(new Point(left, top), new Size(_selectionAreaVisualizer.Width, _selectionAreaVisualizer.Height));
                foreach (var ui in _pad.Canvas.Children.OfType<ISketchItemUI>().Where((x) => !(x.Model is ConnectorModel)))
                {
                    var model = ui.Model as ConnectableBase;
                    ui.IsMarked = selectionArea.Contains(model.Bounds);
                }
            }
            StopOperation(true);

        }

        void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var start = e.GetPosition(_pad.Canvas);
            _startPoint = start;
            _selectionAreaVisualizer = new Rectangle()
            {
                Width = 5,
                Height = 5,
                MinHeight = 5,
                MinWidth = 5,
                Stroke = Brushes.Black,
                StrokeThickness = 0.5,
                StrokeDashArray = new DoubleCollection( new double[] { 5, 5 })
            };
           
            Canvas.SetLeft(_selectionAreaVisualizer, _startPoint.X);
            Canvas.SetTop(_selectionAreaVisualizer, _startPoint.Y);
            _pad.Canvas.MouseLeftButtonUp += MouseLeftButtonUp;
            _pad.Canvas.MouseMove += MouseMove;
            _pad.Canvas.Children.Add(_selectionAreaVisualizer);
            _pad.Canvas.CaptureMouse();
            _pad.Canvas.Focus();
        }

        void MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(_pad.Canvas);
            var dx = Math.Abs(_startPoint.X - pos.X);
            var dy = Math.Abs(_startPoint.Y - pos.Y);
            _selectionAreaVisualizer.Width = Math.Max(dx, _selectionAreaVisualizer.MinWidth);
            _selectionAreaVisualizer.Height = Math.Max(dy, _selectionAreaVisualizer.MinHeight);
            if (pos.X < _startPoint.X)
            {
                Canvas.SetLeft(_selectionAreaVisualizer, pos.X);
            }
            
            if (pos.Y < _startPoint.Y )
            {
                Canvas.SetTop(_selectionAreaVisualizer, pos.Y);
            }
            
            _pad.Canvas.BringIntoView(new Rect(pos, new Size(1, 1)));
            
            _selectionAreaVisualizer.InvalidateVisual();
        }

        public void StopOperation(bool ok)
        {
            if (_selectionAreaVisualizer != null)
            {
                _pad.Canvas.Children.Remove(_selectionAreaVisualizer);
                _selectionAreaVisualizer = null;
            }
            if (!ok)// a new handler is beeing installed!
            {
                _pad.Canvas.KeyDown -= KeyDown;
                _pad.Canvas.MouseLeftButtonDown -= MouseLeftButtonDown;
            }
            _pad.Canvas.ReleaseMouseCapture();
            _pad.Canvas.Focus();
        }

        void MoveMarked(double dx, double dy)
        {
            var transform = new TranslateTransform(dx, dy);
            foreach (var ui in _pad.MarkedItems)
            {
                ui.Model.Move(transform);
            }
        }
    }
}
