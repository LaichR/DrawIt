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

        ISketchItemDisplay _pad;
        Point _startPoint;
        Rectangle _selectionAreaVisualizer;
        bool _selecting = false;

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
                var selectionArea = new Rect(_startPoint, new Size(_selectionAreaVisualizer.Width, _selectionAreaVisualizer.Height));
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
            _selectionAreaVisualizer = new Rectangle();
            _selectionAreaVisualizer.Width = 5;
            _selectionAreaVisualizer.Height = 5;

            _selectionAreaVisualizer.MinHeight = 5;
            _selectionAreaVisualizer.MinWidth = 5;
            _selectionAreaVisualizer.Stroke = Brushes.Black;
            _selectionAreaVisualizer.StrokeThickness = 0.5;
            _selectionAreaVisualizer.StrokeDashArray = new DoubleCollection(new double[] { 5, 5 });
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
            if (pos.X > _startPoint.X + _selectionAreaVisualizer.MinWidth)
            {
                _selectionAreaVisualizer.Width = pos.X - _startPoint.X;
            }
            if (pos.Y > _startPoint.Y + _selectionAreaVisualizer.MinHeight)
            {
                _selectionAreaVisualizer.Height = pos.Y - _startPoint.Y;
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
