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
using UI.Utilities.Controls.ShowError.ViewModel;

namespace UI.Utilities.Controls.ShowError.View
{

    
    /// <summary>
    /// Interaction logic for ShowErrorDlg.xaml
    /// </summary>
    /// 
    public partial class ShowErrorDlg : Window
    {

        readonly ViewModel.ViewModel _model;

        public ShowErrorDlg(string title, Exception ex, bool showStackTrace)
        {
            //InitializeComponent();
            _model = new ViewModel.ViewModel(title, ex, showStackTrace, () => this.Close());
            DataContext = _model;
        }
    }
}
