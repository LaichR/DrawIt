using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Utilities
{
    /// <summary>
    /// Helper class to decorate actions that are used within a delegate command
    /// 
    /// </summary>

    public delegate bool DecoratedAction();
    public delegate bool DecoratedAction<in T>(T obj);

    public class ActionDecorator
    {
        public static DecoratedAction DecorateCatchAndShowException(string title, Action action, bool showStackTrace = true)
        {
            return () =>
            {
                try
                {
                    action();
                    return true;
                }
                catch (Exception e)
                {
                    var dlg = new UI.Utilities.Controls.ShowError.View.ShowErrorDlg(
                        title, e, showStackTrace);
                    dlg.ShowDialog();
                    return false;
                }
            };
        }

        public static DecoratedAction<T> DecorateCatchAndShowException<T>(string title, Action<T> action, bool showStackTrace = true)
        {
            return (x) =>
            {
                try
                {
                    action(x);
                    return true;
                }
                catch (Exception e)
                {
                    var dlg = new UI.Utilities.Controls.ShowError.View.ShowErrorDlg(
                        title, e, showStackTrace);
                    dlg.ShowDialog();
                    return false;
                }
            };
        }
    }
}
