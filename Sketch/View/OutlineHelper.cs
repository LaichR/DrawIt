using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Sketch.Helper.Binding;
using Sketch.Helper.UiUtilities;
using Sketch.View.CustomControls;

namespace Sketch.View
{
    internal static class OutlineHelper
    {
        public static ContextMenu InitContextMenu(IEnumerable<ICommandDescriptor> commands)
        {
            var contextMenu = new ContextMenu();
            foreach (var c in commands)
            {
                var m = new MenuItem
                {
                    Header = c.Name,
                    Command = c.Command,
                    Icon = c.Bitmap != null ?
                            new BitmapImage { Source = ToBitmapSource.Bitmap2BitmapSource(c.Bitmap) } :
                            null
                };
                contextMenu.Items.Add(m);
            };

            return contextMenu;

        }
    }
}
