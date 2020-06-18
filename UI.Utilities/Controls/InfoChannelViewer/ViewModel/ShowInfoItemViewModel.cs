using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using Bluebottle.Base.Configuration;

namespace Bluebottle.Base.Controls.InfoChannelViewer.View
{
    [ConfigurableEntity("ShowInfoItemDialog")]
    internal class ShowInfoItemViewModel : BindableBase
    {
        string _errorMsg = null;
        string _dlgTitle;
        double _left;
        double _top;
        DelegateCommand _copyToClipboardCmd;
        DelegateCommand _closeDlg;
        ConfigurationContainer _configContainer;

        public ShowInfoItemViewModel(string title, string message, Action closeDlg)
        {
            _dlgTitle = title;
            Message = message;
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

        public string Message
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

        private void CopyToClipboard()
        {
            System.Windows.Clipboard.SetText(_errorMsg);
        }
    }
}
