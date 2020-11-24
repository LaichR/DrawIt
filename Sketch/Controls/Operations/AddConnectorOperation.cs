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
using Sketch.Utilities;
using Sketch.Types;
using System.Windows.Media;
using System.Reflection;

namespace Sketch.Controls
{

    internal class AddConnectorOperation : IEditOperation
    {
        readonly ConnectablePairSelector _selector;
        readonly ISketchItemDisplay _pad;
        readonly ConnectableBase _from;
        readonly ContextMenu _oldContextMenue;
        bool _done = false;

        public AddConnectorOperation(ISketchItemDisplay pad, ConnectableBase from, Point p)
        {
            _from = from;
            _pad = pad;
            //
            // at this point it is not sufficient to consider only the bounds
            // in case of a lengthy object, we would like to place that start point close to where 
            // the user pressed the button!
            // hence we should query the from object for a decent start point
            //
            var start = p;// ConnectorUtilities.ComputeCenter(from.Bounds);
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
            Point endPointHint = e.GetPosition(_pad.Canvas);
            var factory = _pad.ItemFactory;
            
            ISketchItemUI sketchItemUI = _pad.GetItemAtPoint(endPointHint);
            
            if (sketchItemUI != null)
            {
                
                if (sketchItemUI.Model is ConnectableBase)
                {
                    var to = sketchItemUI.Model as ConnectableBase;

                    var connectorModel = factory.CreateConnector(
                        factory.SelectedForCreation,
                        _from, to, _selector.Start
                        , endPointHint,
                        _pad );
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
                               double dx = 0; double dy = 0;
                               var angle = Vector.AngleBetween(new Vector(1,0),
                                   new Vector(_selector.Start.X-endPointHint.X, _selector.Start.Y- endPointHint.Y));

                               if (angle < 0) angle += 360.0;

                               var connectable = fac.CreateConnectableItem(endPointHint);
                               
                               if( angle >= 0 && angle < 45)
                               {
                                   dy = -GetDefaultHeight(connectable) / 2;
                                   dx = -GetDefaultWidth(connectable);
                               }
                               else if (angle  >= 45 && angle < 135)
                               {
                                   dx = -GetDefaultWidth(connectable)/2;
                                   dy = -GetDefaultHeight(connectable);
                               }
                               else if( angle >= 135 && angle < 225 )
                               {
                                   dy = -GetDefaultHeight(connectable) / 2;
                                   
                               }
                               else
                               {
                                   
                                   dx = -GetDefaultWidth(connectable) / 2;
                               }

                               connectable.Move(new TranslateTransform(dx, dy));
                               if (connectable != null)
                               {
                                   _pad.SketchItems.Add(connectable);
                                   var connector = factory.CreateConnector(
                                       selectedForCreation,
                                        _from, connectable
                                       , _selector.Start
                                       , endPointHint,
                                       _pad );
                                   _pad.SketchItems.Add(connector);
                                   
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

        static double GetDefaultWidth(object obj)
        {
            var t = obj.GetType();
            var f = t.GetField("DefaultWidth", System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic|BindingFlags.Static);
            if( f != null )
            {
                return Convert.ToDouble(f.GetValue(null));
            }
            return ConnectableBase.DefaultWidth;
        }

        static double GetDefaultHeight(object obj)
        {
            var t = obj.GetType();
            var f = t.GetField("DefaultHeight", System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic | BindingFlags.Static);
            if (f != null)
            {
                return Convert.ToDouble(f.GetValue(null));
            }
            return ConnectableBase.DefaultWidth;
        }

    }
    
}
