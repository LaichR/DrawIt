using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Bluebottle.Base.Controls
{
    public static class AttacheErrorDialog
    {

        public static void ShowBindingError( TextBox textBox)
        {
            ShowBindingError(textBox, TextBox.TextProperty);
        }

        public static void ShowBindingError( ComboBox comboBox)
        {
            ShowBindingError(comboBox, ComboBox.SelectedItemProperty);
            ShowBindingError(comboBox, ComboBox.ItemsSourceProperty);
        }

        public static void ShowBindingError( DataGrid dataGrid )
        {
            dataGrid.PreparingCellForEdit += PreparingDataGridCellForEdit;
        }

        public static void ShowBindingError(Control element, DependencyProperty property)
        {
            var bindingExpression = element.GetBindingExpression(property);
            
            if (bindingExpression != null)
            {
                if (bindingExpression.ParentBinding.UpdateSourceExceptionFilter == null)
                {
                    bindingExpression.ParentBinding.UpdateSourceExceptionFilter =
                        new System.Windows.Data.UpdateSourceExceptionFilterCallback(ShowUpdateException);
                }
            }
        }

        static object ShowUpdateException(object bindExpression, Exception exception)
        {
            var binding = bindExpression;
            var dlg = new Bluebottle.Base.Controls.ShowError.View.ShowErrorDlg("Binding exception", exception, true);
            dlg.ShowDialog();
            return exception;
        }

        static void PreparingDataGridCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            var textBox = e.EditingElement as TextBox;
            if (textBox != null)
            {
                Bluebottle.Base.Controls.AttacheErrorDialog.ShowBindingError(textBox);
                return;
            }

            var comboBox = e.EditingElement as ComboBox;
            if (comboBox != null)
            {
                Bluebottle.Base.Controls.AttacheErrorDialog.ShowBindingError(comboBox);
                return;
            }
        }
    }
}
