using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;


namespace UI.Utilities.Controls
{

    public class EditableTextBlock: TextBox
    {
        public static readonly DependencyProperty EditModeOnProperty =
            DependencyProperty.Register("EditModeOn", typeof(bool), typeof(EditableTextBlock),
                new PropertyMetadata(OnEditModeOnChanged));


        public EditableTextBlock()
            :base()
        {
            //AttacheErrorDialog.ShowBindingError(this);
            IsReadOnly = true;
            Focusable = false;
        }


        public bool EditModeOn
        {
            get { return (bool)GetValue(EditModeOnProperty); }
            set { SetValue(EditModeOnProperty, value); }
        }

        

        protected override Size MeasureOverride(Size constraint)
        {
            var size = base.MeasureOverride(constraint);
            size.Height += 6;
            return size;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            EditModeOnStyle();
            IsReadOnly = false;
            this.SelectAll();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            IsReadOnly = true;
            EditModeOffStyle();
            base.OnLostFocus(e);
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            EditModeOffStyle();
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if( e.Key == System.Windows.Input.Key.Enter )
            {
                this.LockCurrentUndoUnit();
                EditModeOn = false;
                e.Handled = false;
                return;
            }
            else if( e.Key == System.Windows.Input.Key.Escape)
            {
                this.Undo();
                var limit = this.UndoLimit;
                this.UndoLimit = 1;
                this.UndoLimit = limit;
                EditModeOn = false;
            }
            base.OnKeyDown(e);
        }

        void EditModeOffStyle()
        {
            
            BorderBrush = Brushes.White;
            var parent = VisualTreeHelper.GetParent(this);
            if( parent != null)
            {
                Background = (Brush)parent.GetValue(BackgroundProperty);
            }
            BorderThickness = new Thickness(0);  
            
        }

        void EditModeOnStyle()
        {
            BorderBrush = Brushes.Blue;            
            this.BorderThickness = new Thickness(2);
            Background = Brushes.LightGray;
        }

        private static void OnEditModeOnChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            var tb = source as EditableTextBlock;
            var editModeOnNew = Convert.ToBoolean(args.NewValue);
            var editModeOnOld = Convert.ToBoolean(args.OldValue);
            if (tb != null && editModeOnNew != editModeOnOld)
            {
                tb.Focusable = editModeOnNew;
                if (editModeOnNew)
                {
                    tb.Focus();
                }
            }
        }
    }
}
