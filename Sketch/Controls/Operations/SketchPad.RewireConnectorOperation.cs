using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using Sketch.Models;
using Sketch.Types;
using Sketch.Interface;

namespace Sketch.Controls
{
    public partial class SketchPad
    {
        internal class RewireConnectorOperation : IEditOperation
        {
            ConnectablePairSelector _selector;
            SketchPad _pad;
            ConnectorModel _connector;

            public RewireConnectorOperation(SketchPad pad, ConnectorModel model, Point p)
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
                _pad.Children.Add(_selector);
                _selector.Visibility = Visibility.Visible;
                foreach (var ch in _pad.Children.OfType<IGadgetUI>())
                {
                    ch.Disable();
                }
                _pad.MouseMove += HandleMouseMove;
                _pad.MouseDown += HandleMouseDown;
                _pad.KeyDown += HandleKeyDown;
            }

            void HandleKeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Escape)
                {
                    _pad.EndOperation();
                    _connector.RestoreConnectionEnd();
                    e.Handled = true;
                }
            }

            void HandleMouseDown(object sender, MouseButtonEventArgs e)
            {
                e.Handled = true;
                var inputElem = _pad.InputHitTest(e.GetPosition(_pad)) as IGadgetUI;
                if (inputElem != null)
                {
                    if (inputElem.Model is ConnectableBase)
                    {
                        var ending2 = inputElem.Model as ConnectableBase;
                        if( _connector.To == null )
                        {
                            _connector.To = ending2;
                        }
                        else
                        {
                            _connector.From = ending2;
                        }
                    }
                    _pad.EndOperation();
                    
                }
            }

            void HandleMouseMove(object sender, MouseEventArgs e)
            {
                _selector.ComputePath(e.GetPosition(_pad));
                e.Handled = true;
            }


            public void StopOperation(bool commit)
            {
                _pad.MouseMove -= HandleMouseMove;
                _pad.MouseDown -= HandleMouseDown;
                _pad.KeyDown -= HandleKeyDown;

                _pad.Children.Remove(_selector);
                foreach (var ch in _pad.Children.OfType<IGadgetUI>())
                {
                    ch.Enable();
                }
            }
        }
    }
}
