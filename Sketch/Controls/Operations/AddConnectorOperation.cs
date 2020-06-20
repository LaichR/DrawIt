using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using Prism.Commands;
using Sketch.Models;
using Sketch.Interface;

namespace Sketch.Controls
{

    internal class AddConnectorOperation : IEditOperation
    {
        ConnectablePairSelector _selector;
        ISketchItemDisplay _pad;
        ConnectableBase _from;
        ContextMenu _oldContextMenue;
        bool _done = false;

        public AddConnectorOperation(ISketchItemDisplay pad, ConnectableBase from, Point p)
        {
            _from = from;
            _pad = pad;
            var start = ConnectorUtilities.ComputeCenter(from.Bounds);
            _selector = new ConnectablePairSelector(start, p);
            _pad.Canvas.Children.Add(_selector);
            _selector.Visibility = Visibility.Visible;
            _pad.SetSketchItemEnable(false);

            _oldContextMenue = _pad.Canvas.ContextMenu;
            _pad.Canvas.MouseMove += HandleMouseMove;
            _pad.Canvas.MouseDown += HandleMouseDown;
            _pad.Canvas.KeyDown += HandleKeyDown;
            _pad.TakeSnapshot();

        }

        void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _pad.DropSnapshot();
                _pad.EndEdit();
                e.Handled = true;
            }
        }



        void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Point p = e.GetPosition(_pad.Canvas);
            var factory = ModelFactoryRegistry.Instance.GetSketchItemFactory();
            var inputElem = _pad.Canvas.InputHitTest(e.GetPosition(_pad.Canvas)) as ISketchItemUI;
            if (inputElem != null)
            {
                if (inputElem.Model is ConnectableBase)
                {
                    var to = inputElem.Model as ConnectableBase;

                    var connectorModel = factory.CreateConnector(
                        factory.SelectedForCreation,
                        ConnectionType.AutoRouting, _from, to);
                    _pad.SketchItems.Add(connectorModel);
                }
                _pad.EndEdit();
            }
            else
            {
                _pad.Canvas.ContextMenu = new ContextMenu();
                var selectedForCreation = factory.SelectedForCreation;
                var factoryList = factory.GetConnectableFactories(
                    selectedForCreation
                    ).OrderByDescending((x) => x.LastCalled);
                foreach (var fac in factoryList)
                {
                    _pad.Canvas.ContextMenu.Items.Add(new MenuItem()
                    {
                        Icon = new UI.Utilities.Controls.BitmapImage { ImageBitmap = fac.Bitmap },
                        Header = new Label() { Content = fac.Name },
                        ToolTip = fac.ToolTip,
                        Command = new DelegateCommand(() =>
                           {
                               var connectable = fac.CreateConnectableItem(p);
                               if (connectable != null)
                               {
                                   _pad.SketchItems.Add(connectable);
                                   _pad.SketchItems.Add(factory.CreateConnector(
                                       selectedForCreation,
                                       ConnectionType.AutoRouting, _from, connectable));
                                   _pad.EndEdit();
                               }
                           })
                    }
                    );
                }
                _pad.Canvas.ContextMenu.IsOpen = true;

            }
        }

        void HandleMouseMove(object sender, MouseEventArgs e)
        {
            _selector.ComputePath(e.GetPosition(_pad.Canvas));
            e.Handled = true;
        }


        public void StopOperation(bool commit)
        {
            if (!_done)
            {
                _done = true;
                _pad.Canvas.MouseMove -= HandleMouseMove;
                _pad.Canvas.MouseDown -= HandleMouseDown;
                _pad.Canvas.KeyDown -= HandleKeyDown;

                _pad.Canvas.Children.Remove(_selector);

                _pad.SetSketchItemEnable(true);

                _pad.Canvas.ContextMenu = _oldContextMenue;
            }
        }
    }
    
}
