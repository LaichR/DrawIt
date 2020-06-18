using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;


namespace Sketch.Controls
{
    internal class NopHandler : IEditOperation
    {
        SketchPad _pad;

        public NopHandler(SketchPad pad)
        {
            _pad = pad;
            _pad.MouseDown += _pad_MouseDown;
        }

        void _pad_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _pad.Focus();
        }
        public void StopOperation(bool ok) { _pad.MouseDown -= _pad_MouseDown; }
       
    }
}
