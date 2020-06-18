using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using Sketch.Models;

namespace Sketch.Controls
{
    public partial class SketchPad
    {
        internal class AddGadgetOperation : IEditOperation
        {
            SketchPad _pad;

            public AddGadgetOperation(SketchPad pad)
            {
                _pad = pad;
                _pad.Focus();
                _pad.MouseDown += HandleMouseDown;
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
                _pad.Focus();
                var p = e.GetPosition(_pad);

                p.X = (p.X / _pad.Grid) * _pad.Grid;
                p.Y = (p.Y / _pad.Grid) * _pad.Grid;
                var factory = ModelFactoryRegistry.Instance.GetSketchItemFactory();
                var cm = ModelFactoryRegistry.Instance.GetSketchItemFactory().CreateConnectableSketchItem(factory.SelectedForCreation,
                    p);
                if (cm != null)
                {
                    _pad.TakeSnapshot();
                    _pad.SketchItems.Add(cm);
                }
                //_pad.UpdateLayout();
                e.Handled = true;
            }

           
            public void StopOperation(bool commit) 
            {
                _pad.MouseDown -= HandleMouseDown;
            }

        }
    }
}
