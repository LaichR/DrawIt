using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bluebottle.Base.Controls
{
    public class AuotSelectTextbox : TextBox
    {
        private bool _catchFocusOnVisible = false;
        public bool CatchFocusOnVisible 
        {
            get 
            { return _catchFocusOnVisible; }
            set
            {
                _catchFocusOnVisible = value;
            }
        }

        bool _firstTextEnteredProgrammatically = true;

        public AuotSelectTextbox()
        {
        }

        /// <summary>
        /// Selects the entire text if the text is entered by setting the text property.
        /// This overcomes the problem that the event which handle the selection of the text
        /// fires before that the framework loads the text into the field, resulting in an empty selection.
        /// If the first text is entered by the user manually, the selection will not be done.
        /// </summary>
        /// <see cref="OnKeyDown"/>
        /// <param name="e"></param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (_firstTextEnteredProgrammatically)
            {
                _firstTextEnteredProgrammatically = false;
                if (this.IsFocused)
                    this.SelectAll();
            }
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            _firstTextEnteredProgrammatically = false;
            base.OnKeyDown(e);
        }
        
        /// <summary>
        /// Set the Focus to this control the first time that the control is clicked inside the text area.
        /// This is a work-around to select the all text in the control after a click.
        /// </summary>
        /// <see cref="http://www.intertech.com/Blog/how-to-select-all-text-in-a-wpf-textbox-on-focus/"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnPreviewMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (!this.IsKeyboardFocusWithin)
            {
                if (e.OriginalSource.GetType().Name == "TextBoxView")
                {
                    this.Focus();
                    e.Handled = true;
                }
            }
        }
       
        /// <summary>
        /// If the property CatchFocusOnVisible is set, this sets the control in Focus when the control becomes visible.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CatchFocusOnVisible && 
                e.Property == System.Windows.Controls.TextBox.IsVisibleProperty &&
                (bool)e.NewValue)
            {
                this.Focus();
            }
         
            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// Selects the entire text any time that the control get the foucs.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            this.SelectAll();
        }
    }
}
