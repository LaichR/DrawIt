using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sketch.Helper.Binding
{
    public class DelegateCommand : ICommand
    {

        readonly Action _exectueDelegate;
        readonly Func<bool> _canExecute;

        public DelegateCommand(Action delegate_, Func<bool> canExecute_=null)
        {
            _exectueDelegate = delegate_;
            _canExecute = canExecute_;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object _1)
        {
            if (_canExecute != null) return _canExecute();
            return true;
        }

        public void Execute(object _1)
        {
            _exectueDelegate();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
