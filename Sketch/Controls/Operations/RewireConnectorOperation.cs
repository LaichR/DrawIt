using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using Sketch.Models;
using Sketch.Helper;
using Sketch.Interface;

namespace Sketch.Controls
{

    internal class RewireConnectorOperation : IEditOperation
    {
        readonly ConnectablePairSelector _selector;
        readonly ISketchItemDisplay _pad;
        readonly ConnectorModel _connector;

        public RewireConnectorOperation(ISketchItemDisplay pad, ConnectorModel model, Point p)
        {
            _connector = model;
            _pad = pad;

            IBoundedItemModel ending = model.To;
            if (ending == null)
            {
                ending = model.From;
            }
            var start = ConnectorUtilities.ComputeCenter(ending.Bounds);
            _selector = new ConnectablePairSelector(start, p);
            _pad.Canvas.Children.Add(_selector);
            _selector.Visibility = Visibility.Visible;
            foreach (var ch in _pad.Canvas.Children.OfType<ISketchItemUI>())
            {
                ch.Disable();
            }
            _pad.Canvas.MouseMove += HandleMouseMove;
            _pad.Canvas.MouseDown += HandleMouseDown;
            _pad.Canvas.KeyDown += HandleKeyDown;
        }

        void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _pad.EndEdit();
                _connector.RestoreConnectionEnd();
                e.Handled = true;
            }
        }

        void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            
            var inputElem = _pad.GetItemAtPoint(e.GetPosition(_pad.Canvas)) as ISketchItemUI;
            if (inputElem != null)
            {
                if (inputElem.Model is ConnectableBase)
                {
                    var ending2 = inputElem.Model as ConnectableBase;
                    if (_connector.To == null)
                    {
                        _connector.To = ending2;
                    }
                    else
                    {
                        _connector.From = ending2;
                    }
                }
                _pad.EndEdit();

            }
        }

        void HandleMouseMove(object sender, MouseEventArgs e)
        {
            _selector.ComputePath(e.GetPosition(_pad.Canvas));
            e.Handled = true;
        }


        public void StopOperation(bool commit)
        {
            _pad.Canvas.MouseMove -= HandleMouseMove;
            _pad.Canvas.MouseDown -= HandleMouseDown;
            _pad.Canvas.KeyDown -= HandleKeyDown;

            _pad.Canvas.Children.Remove(_selector);
            foreach (var ch in _pad.Canvas.Children.OfType<ISketchItemUI>())
            {
                ch.Enable();
            }
        }
    }
}
