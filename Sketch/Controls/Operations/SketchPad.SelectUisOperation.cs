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
using Sketch.Models;

namespace Sketch.Controls
{
    public partial class SketchPad
    {
        internal class SelectUisOperation : IEditOperation
        {

            SketchPad _pad;
            Point _startPoint;
            Rectangle _selectionAreaVisualizer;
            bool _selecting = false;

            public SelectUisOperation(SketchPad pad)
            {
                _pad = pad;
                
                _pad.MouseLeftButtonDown += MouseLeftButtonDown;
                _pad.KeyDown += KeyDown;
            }

            void KeyDown(object sender, KeyEventArgs e)
            {
                e.Handled = true;
                //if( _pad._S)
                switch(e.Key)
                {
                    case Key.Left:
                        _pad.MoveSelected(-1 * GridSize, 0);
                        break;
                    case Key.Right:
                        _pad.MoveSelected(1 * GridSize, 0);
                        break;
                    case Key.Up:
                        _pad.MoveSelected(0, -1 * GridSize);
                        break;
                    case Key.Down:
                        _pad.MoveSelected(0, 1 * GridSize);
                        break;
                    default:
                        e.Handled = false;
                        break;
                }
            }

            void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
               
                _pad.MouseLeftButtonUp -= MouseLeftButtonUp;
                _pad.MouseMove -= MouseMove;
                var ok = e.RightButton == MouseButtonState.Released;
                if (ok)
                {
                    if (_pad._selectedGadget != null)
                    {
                        _pad._selectedGadget.IsSelected = false;
                    }
                    var selectionArea = new Rect(_startPoint, new Size(_selectionAreaVisualizer.Width, _selectionAreaVisualizer.Height));
                    foreach (var ui in _pad._activeUis.Where((x) => !(x.Model is ConnectorModel)))
                    {
                        var model = ui.Model as ConnectableBase;
                        ui.IsMarked = selectionArea.Contains(model.Bounds);
                        // the first ui element will be the selected gadget
                        //if( _pad._selectedGadget == null && ui.IsSelected)
                        //{
                        //    _pad._selectedGadget = ui;
                        //}
                    }
                }
                StopOperation(true);
                
            }

            void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var start = e.GetPosition(_pad);
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
                _pad.MouseLeftButtonUp += MouseLeftButtonUp;
                _pad.MouseMove += MouseMove;
                _pad.Children.Add(_selectionAreaVisualizer);
                _pad.CaptureMouse();
                _pad.Focus();
            }

            void MouseMove(object sender, MouseEventArgs e)
            {
                var pos = e.GetPosition(_pad);
                if( pos.X > _startPoint.X + _selectionAreaVisualizer.MinWidth )
                {
                    _selectionAreaVisualizer.Width = pos.X - _startPoint.X;
                }
                if( pos.Y > _startPoint.Y + _selectionAreaVisualizer.MinHeight)
                {
                    _selectionAreaVisualizer.Height = pos.Y - _startPoint.Y;
                }
                _pad.BringIntoView(new Rect(pos, new Size(1, 1)));
                _selectionAreaVisualizer.InvalidateVisual();
            }

            public void StopOperation(bool ok)
            {
                if (_selectionAreaVisualizer != null)
                {
                    _pad.Children.Remove(_selectionAreaVisualizer);
                    _selectionAreaVisualizer = null;
                }
                if (!ok)// a new handler is beeing installed!
                {
                    _pad.KeyDown -= KeyDown;
                    _pad.MouseLeftButtonDown -= MouseLeftButtonDown;
                }
                _pad.ReleaseMouseCapture();
                _pad.Focus();
            }
        }
    }
}
