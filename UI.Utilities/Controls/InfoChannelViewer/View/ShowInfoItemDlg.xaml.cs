using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Bluebottle.Base.Controls.InfoChannelViewer.ViewModel;

namespace Bluebottle.Base.Controls.InfoChannelViewer.View
{

    
    /// <summary>
    /// Interaction logic for ShowErrorDlg.xaml
    /// </summary>
    /// 
    public partial class ShowInfoItemDlg : Window
    {

        ShowInfoItemViewModel _model;

        public ShowInfoItemDlg(string title, string message, Level level )
        {
            InitializeComponent();
            _model = new ShowInfoItemViewModel(title, message, () => this.Close());
            DataContext = _model;
        }
    }
}
