using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Controls;

namespace Bluebottle.Base
{
    public class ExtendedBinding : System.Windows.Data.Binding
    {
        public ExtendedBinding()
            : base()
        {
            InitializeBinding();
        }

        public ExtendedBinding(string path)
            : base(path)
        {
            InitializeBinding();
        }

        private void InitializeBinding()
        {
            UpdateSourceExceptionFilter = _filterCallback;
            ValidationRules.Add(_exceptionRule);
        }

        private static UpdateSourceExceptionFilterCallback _filterCallback = new UpdateSourceExceptionFilterCallback(ExceptionHandler);

        private static ExceptionValidationRule _exceptionRule = new ExceptionValidationRule();

        private static object ExceptionHandler(object bindingExpression, Exception exception)
        {
            var dlg = new Bluebottle.Base.Controls.ShowError.View.ShowErrorDlg("Binding error", exception, true);
            dlg.ShowDialog();

            (bindingExpression as BindingExpression).UpdateTarget();

            //return null;

            // if you want a validation error to appear in the UI,
            return new ValidationError(((BindingExpression)bindingExpression).ParentBinding.ValidationRules[0], bindingExpression, exception.Message, exception);
        }
    }


    public delegate void PropertySetAction<in T>(T obj);

    public class Property
    {
        public static void Set(Action set)
        {
            ActionDecorator.DecorateCatchAndShowException("Property set unhandled excpetion.", set)();
        }

        public static void Set<T>(T value, Action<T> set)
        {
            ActionDecorator.DecorateCatchAndShowException<T>("Property set unhandled excpetion.", set)(value);
        }
    }
}
