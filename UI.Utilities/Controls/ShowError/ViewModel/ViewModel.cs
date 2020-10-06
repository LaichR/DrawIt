using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using UI.Utilities.Configuration;

namespace UI.Utilities.Controls.ShowError.ViewModel
{
    [ConfigurableEntity("ErrorDialog")]
    internal class ViewModel: BindableBase
    {
        string _errorMsg = null;
        string _dlgTitle;
        double _left;
        double _top;
        readonly DelegateCommand _copyToClipboardCmd;
        readonly DelegateCommand _closeDlg;
        readonly ConfigurationContainer _configContainer;

        public ViewModel(string title, Exception ex, bool showStackTrace, Action closeDlg)
        {
            ErrorMsg = FormatException(ex, showStackTrace);
            _dlgTitle = title;
            _copyToClipboardCmd = new DelegateCommand(CopyToClipboard);
            _configContainer = new ConfigurationContainer(this);
            _closeDlg = new DelegateCommand(
                () =>
                {
                    _configContainer.SaveConfig();
                    closeDlg();
                });
            
            _configContainer.LoadConfig();
        }

        [ConfigurableProperty(1)]
        public double Top
        {
            get { return _top; }
            set { SetProperty<double>(ref _top, value); }
        }

        [ConfigurableProperty(2)]
        public double Left
        {
            get { return _left; }
            set { SetProperty<double>(ref _left, value); }
        }

        public string DlgTitle
        {
            get
            {
                return _dlgTitle;
            }
            set
            {
                SetProperty<string>(ref _dlgTitle, value);
            }
        }

        public string ErrorMsg
        {
            get { return _errorMsg; }
            set { SetProperty<string>(ref _errorMsg, value); }
        }

        public DelegateCommand CmdCopyToClipboard
        {
            get { return _copyToClipboardCmd; }
        }

        public DelegateCommand CmdClose
        {
            get { return _closeDlg; }
        }

        string FormatException(Exception e, bool showStackTrace)
        {
            StringBuilder str = new StringBuilder();
            if (e == null)
                return "";
            var stackTrace = e.StackTrace;
            while (e != null)
            {
                if (str.Length > 0)
                    str.Append(string.Format("\n\n\tInner -> {0}", e.Message));
                else
                    str.Append(e.Message);

                e = e.InnerException;
            }
            if (showStackTrace)
            {
                str.Append("\n\nStack trace:\n");
                str.Append(stackTrace);
            }
            return str.ToString();
        }

        private void CopyToClipboard()
        {
            System.Windows.Clipboard.SetText(_errorMsg);
        }
    }
}
