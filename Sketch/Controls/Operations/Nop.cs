using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using Sketch.Interface;

namespace Sketch.Controls
{
    internal class NopHandler : IEditOperation
    {
        ISketchItemDisplay _pad;

        public NopHandler(ISketchItemDisplay pad)
        {
            _pad = pad;
            _pad.Canvas.MouseDown += _pad_MouseDown;
        }

        void _pad_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _pad.Canvas.Focus();
        }
        public void StopOperation(bool ok) { _pad.Canvas.MouseDown -= _pad_MouseDown; }
       
    }
}
