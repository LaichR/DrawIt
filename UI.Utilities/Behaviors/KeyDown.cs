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
    public static class KeyDown
    {
        public static readonly DependencyProperty EnterProperty =
            DependencyProperty.RegisterAttached("Enter",
            typeof(ICommand),
            typeof(KeyDown),
            new UIPropertyMetadata((x,y)=>OnBindingChanged(OnEnter, x,y)));

        public static readonly DependencyProperty EnterArgumentProperty =
            DependencyProperty.RegisterAttached("EnterArgument",
            typeof(object),
            typeof(KeyDown),
            new PropertyMetadata());

        public static readonly DependencyProperty Key2CommandProperty =
            DependencyProperty.RegisterAttached("Key2Command",
                typeof(Dictionary<Key, ICommand>),
                typeof(KeyDown),
                new UIPropertyMetadata((x, y) => OnBindingChanged(OnAnyKeyDown, x, y)));


        public static void SetEnter(DependencyObject target, ICommand value)
        {
            target.SetValue(EnterProperty, value);
        }

        public static ICommand GetEnter(DependencyObject target)
        {
            return target.GetValue(EnterProperty) as ICommand;
        }

        public static void SetEnterArgument(DependencyObject target, object value)
        {
            target.SetValue(EnterArgumentProperty, value);
        }

        public static object GetEnterArgument(DependencyObject target)
        {
            return target.GetValue(EnterArgumentProperty);
        }

        public static void SetKey2Command(DependencyObject target, object value)
        {
            target.SetValue(Key2CommandProperty, value);
        }

        public static object GetKey2Command(DependencyObject target)
        {
            return target.GetValue(Key2CommandProperty);
        }

        private static void OnBindingChanged( KeyEventHandler handler, DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            Control control = target as Control;
            if (control != null)
            {
                // there was no registered command so far
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    Keyboard.AddKeyDownHandler(control, handler);
                    control.PreviewKeyDown += handler;
                    //control.KeyDown += handler;
                }
                // there is no registerd command anymore
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    Keyboard.RemoveKeyDownHandler(control, handler);
                    //control.KeyDown -= handler;
                    control.PreviewKeyDown -= handler;
                }
            }
        }

        private static void OnEnter( object sender, RoutedEventArgs e)
        {
            e.Handled = false;
            Control control = sender as Control;
            if (control == null) return;
            KeyEventArgs args = e as KeyEventArgs;
            ICommand command = null;
            object commandArguments = null;
            if (args.Key == Key.Enter)
            {
                command = (ICommand)control.GetValue(EnterProperty);
                commandArguments = control.GetValue(EnterArgumentProperty);
                command.Execute(commandArguments);
                e.Handled = true;
            }
        }

        private static void OnAnyKeyDown( object sender, KeyEventArgs e)
        {
            e.Handled = false;
            Control control = sender as Control;
            if (control == null) return;
            var key2CommandMap = control.GetValue(Key2CommandProperty) as Dictionary<Key, ICommand>;
            if( key2CommandMap != null)
            {
                ICommand cmd = null;
                if( key2CommandMap.TryGetValue(e.Key, out cmd))
                {
                    if( cmd.CanExecute(null) )
                    {
                        cmd.Execute(null);
                        e.Handled = true;
                        control.Focus();
                    }
                }
            }
        }

    }
}
