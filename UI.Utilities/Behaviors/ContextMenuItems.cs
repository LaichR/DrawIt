using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Bluebottle.Base.Controls;
using Bluebottle.Base.Interfaces;

namespace Bluebottle.Base.Behaviors
{
    public static class ContextMenuItems
    {
        public static readonly DependencyProperty ContextMenuItemsProperty =
            DependencyProperty.RegisterAttached("MenuItems",
            typeof(IList<ICommandDescriptor>),
            typeof(ContextMenuItems),
            new UIPropertyMetadata(ContextMenuItemsChanged));


        public static void SetMenuItems(DependencyObject target, IList<ICommandDescriptor> value)
        {
            target.SetValue(ContextMenuItemsProperty, value);
        }

        public static IList<ICommandDescriptor> GetMenuItems(DependencyObject target)
        {
            return target.GetValue(ContextMenuItemsProperty) as IList<ICommandDescriptor>;
        }

        private static void ContextMenuItemsChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            Control control = target as Control;
            if (control != null)
            {
                var menuItems = e.NewValue as IList<ICommandDescriptor>;
                if (menuItems != null)
                {
                    if (control.ContextMenu == null)
                    {
                        var contextMenu = new ContextMenu();
                        foreach (var i in menuItems)
                        {
                            var m = new MenuItem
                            {
                                Header = i.Name,
                                Command = i.Command,
                                Icon = i.Bitmap != null? 
                                        new BitmapImage{Source=ToBitmapSource.Bitmap2BitmapSource(i.Bitmap)} : 
                                        null
                            };
                            contextMenu.Items.Add(m);
                        }
                        control.ContextMenu = contextMenu;
                    }
                }                
            }
        }

    }
}
