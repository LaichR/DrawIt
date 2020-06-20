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
using Sketch.Interface;

namespace Sketch.Controls
{

    internal class AddConnectableItemOperation : IEditOperation
    {
        ISketchItemDisplay _pad;

        public AddConnectableItemOperation(ISketchItemDisplay pad)
        {
            _pad = pad;
            _pad.Canvas.Focus();
            _pad.Canvas.MouseDown += HandleMouseDown;
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
            _pad.Canvas.Focus();
            var p = e.GetPosition(_pad.Canvas);

            p.X = (p.X / SketchPad.GridSize) * SketchPad.GridSize;
            p.Y = (p.Y / SketchPad.GridSize) * SketchPad.GridSize;
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
            _pad.Canvas.MouseDown -= HandleMouseDown;
        }

    }
}

