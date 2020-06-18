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

namespace Bluebottle.Base.Behaviors
{
    public static class MouseDoubleClick
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
            typeof(ICommand),
            typeof(MouseDoubleClick),
            new UIPropertyMetadata(CommandChanged));

        public static readonly DependencyProperty CommandArgumentProperty =
            DependencyProperty.RegisterAttached("CommandArgument",
            typeof(object),
            typeof(MouseDoubleClick),
            new PropertyMetadata());

        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject target)
        {
            return target.GetValue(CommandProperty) as ICommand;
        }

        public static void SetCommandArgument(DependencyObject target, object value)
        {
            target.SetValue(CommandArgumentProperty, value);
        }

        public static object GetCommandArgument(DependencyObject target)
        {
            return target.GetValue(CommandArgumentProperty);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            Control control = target as Control;
            if (control != null)
            {
                // there was no registered command so far
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.MouseDoubleClick += OnMouseDoubleClick;
                }
                // there is no registerd command anymore
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.MouseDoubleClick -= OnMouseDoubleClick;
                }
            }
        }

        private static void OnMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Control control = sender as Control;
            if (control == null) return;
            MouseButtonEventArgs args = e as MouseButtonEventArgs;
            if (args.ClickCount == 1)
            {
                ICommand command = (ICommand)control.GetValue(CommandProperty);
                var argument = control.GetValue(CommandArgumentProperty);
                command.Execute(argument);
                e.Handled = true;
            }
        }


    }
}
