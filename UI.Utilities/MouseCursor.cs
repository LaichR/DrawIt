using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bluebottle.Base
{
    public class WaitCursor : IDisposable
    {
        private Cursor _previousCursor;

        public WaitCursor()
        {
            if (Application.Current == null) //WinForm app, otherwise WPF
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _previousCursor = Mouse.OverrideCursor;
                    Mouse.OverrideCursor = Cursors.Wait;
                });
            }

        }

        public void Dispose()
        {
            if (Application.Current == null) //WinForm app, otherwise WPF
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                   {
                       Mouse.OverrideCursor = _previousCursor;
                   });
            }       
        }
    }
}
