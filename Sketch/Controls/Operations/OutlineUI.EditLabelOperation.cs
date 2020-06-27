using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

using Sketch.Models;
using Sketch.Interface;

namespace Sketch.Controls
{
    public partial class OutlineUI
    {
        internal class EditLabelOperation : IEditOperation
        {
            OutlineUI _owner;
            ISketchItemDisplay _panel;
            
            TextBox _myLabelEditor = new TextBox();

            public EditLabelOperation(OutlineUI owner)
            {
                _owner = owner;
                _panel = _owner._parent;
                var bindingName = _owner.Model.LabelPropertyName;
                var binding = new Binding(bindingName);
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                _myLabelEditor.IsInactiveSelectionHighlightEnabled = true;
                _myLabelEditor.SetBinding(TextBox.TextProperty, binding);
                _myLabelEditor.Visibility = Visibility.Hidden;
                _myLabelEditor.DataContext = _owner._model;
                _myLabelEditor.AcceptsReturn = false;
                _myLabelEditor.KeyDown += HandleEditorKeyDown;
                _myLabelEditor.KeyUp += HandleEditorKeyUp;
                _myLabelEditor.Loaded +=_myLabelEditor_Loaded;
                
                _owner.MouseDown += HandleMouseDown;
                ShowEditor();
            }



            void _myLabelEditor_Loaded(object sender, RoutedEventArgs e)
            {
                
                _myLabelEditor.SelectAll();
            }
            

            void HandleEditorKeyUp( object sender, KeyEventArgs e)
            {
                if( e.Key == Key.LeftShift || e.Key == Key.RightShift)
                {
                    _myLabelEditor.AcceptsReturn = false;
                }
            }

            void HandleEditorKeyDown(object sender, KeyEventArgs e)
            {
                if( e.Key == Key.LeftShift || e.Key == Key.RightShift)
                {
                    _myLabelEditor.AcceptsReturn = true;
                }
                else if (e.Key == Key.Enter && !e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    e.Handled = true;
                    StopOperation(true);
                    e.Handled = false;
                }
                
            }

            void HandleMouseDown(object sender, MouseButtonEventArgs e)
            {
                bool commit = e.RightButton == MouseButtonState.Released;
                StopOperation(commit);
                
                e.Handled = true;
            }

            public void StopOperation(bool commit)
            {
                _owner.MouseDown -= HandleMouseDown;
                _owner.RegisterHandler(null);
                if( commit )
                {
                    _owner.Model.UpdateGeometry();
                }
                HideEditor();
            }

            void ShowEditor()
            {
                if (!_panel.Canvas.Children.Contains(_myLabelEditor))
                {
                    _panel.Canvas.Children.Add(_myLabelEditor);
                }
                Canvas.SetZIndex(_myLabelEditor, 3000);
                Canvas.SetLeft(_myLabelEditor, _owner.LabelArea.Left + 1);
                Canvas.SetTop(_myLabelEditor, _owner.LabelArea.Top + 1);
                _myLabelEditor.Width = Math.Max( 30, _owner.LabelArea.Width - 2);
                _myLabelEditor.MaxWidth = _myLabelEditor.Width;
                _myLabelEditor.Height = _owner.LabelArea.Height;
                _myLabelEditor.AcceptsReturn = true;
                _myLabelEditor.TextWrapping = TextWrapping.Wrap;
                
                _myLabelEditor.Visibility = Visibility.Visible;
                _myLabelEditor.Focus();
                
            }

            void HideEditor()
            {
                if (_panel.Canvas.Children.Contains(_myLabelEditor))
                {
                    _panel.Canvas.Children.Remove(_myLabelEditor);
                }
                _myLabelEditor.Visibility = Visibility.Hidden;
            }


        }
    }
}
