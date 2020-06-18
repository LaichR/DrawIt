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

namespace Bluebottle.Base.Controls.SelectFolder.View
{
    /// <summary>
    /// Interaction logic for SelectFolderDlg.xaml
    /// </summary>
    public partial class SelectFolderDlg : Window
    {
        SelectFolder.ViewModel.SelectFolderViewModel _model;

        public SelectFolderDlg( string title, string root )
            :base()
        {
            InitializeComponent();
            _model = new ViewModel.SelectFolderViewModel(title, root, ()=> this.Close() );
            DataContext = _model;
        }

        public bool IsSelectionOk
        {
            get
            {
                return _model.IsSelectionOk;
            }
        }

        public string SelectedPath
        {
            get
            {
                return _model.SelectedPath;
            }
        }
    }
}
