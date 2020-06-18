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
    

    public partial class SketchPad
    {
        internal class AddConnectorOperation : IEditOperation
        {
            ConnectablePairSelector _selector;
            SketchPad _pad;
            ConnectableBase _from;
            ContextMenu _oldContextMenue;

            public AddConnectorOperation(SketchPad pad, ConnectableBase from, Point p)
            {
                _from = from;
                _pad = pad;
                var start = ConnectorUtilities.ComputeCenter(from.Bounds);
                _selector = new ConnectablePairSelector(start, p);
                _pad.Children.Add(_selector);
                _selector.Visibility = Visibility.Visible;
                _pad.SetOutlineEnable( false );
                
                _oldContextMenue = _pad.ContextMenu;
                _pad.MouseMove += HandleMouseMove;
                _pad.MouseDown += HandleMouseDown;
                _pad.KeyDown += HandleKeyDown;
                _pad.TakeSnapshot();
                
            }

            void HandleKeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Escape)
                {
                    _pad.DropSnapshot();
                    _pad.EndOperation();
                    e.Handled = true;
                }
            }

           

            void HandleMouseDown(object sender, MouseButtonEventArgs e)
            {
                e.Handled = true;
                Point p = e.GetPosition(_pad);
                var factory = ModelFactoryRegistry.Instance.GetSketchItemFactory();
                var inputElem = _pad.InputHitTest(e.GetPosition(_pad)) as IGadgetUI;
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
                    _pad.EndOperation();   
                }
                else
                {                    
                    _pad.ContextMenu = new ContextMenu();
                    var selectedForCreation = factory.SelectedForCreation;
                    var factoryList = factory.GetConnectableFactories(
                        selectedForCreation
                        ).OrderByDescending((x)=>x.LastCalled );
                    foreach (var fac in factoryList)
                    {
                        _pad.ContextMenu.Items.Add(new MenuItem()
                        {
                            Icon = new UI.Utilities.Controls.BitmapImage { ImageBitmap = fac.Bitmap },
                            Header = new Label() { Content = fac.Name },
                            ToolTip = fac.ToolTip,
                            Command = new DelegateCommand( () => 
                                {
                                    var connectable = fac.CreateConnectableItem( p );
                                    if (connectable != null)
                                    {
                                        _pad.SketchItems.Add(connectable);
                                        _pad.SketchItems.Add(factory.CreateConnector(
                                            selectedForCreation,
                                            ConnectionType.AutoRouting, _from, connectable));
                                        _pad.EndOperation();
                                    }
                                } )
                        }
                        );
                    }
                    _pad.ContextMenu.IsOpen = true;
                    
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

                _pad.RegisterHandler(null);
                
                _pad.ContextMenu = _oldContextMenue;
            }
        }
    }
}
