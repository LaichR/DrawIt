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

namespace Bluebottle.Base.Controls.InfoChannelViewer.View
{
    /// <summary>
    /// Interaction logic for InfoChannelViewer.xaml
    /// </summary>
    public partial class InfoChannelViewer : UserControl
    {
        public InfoChannelViewer()
        {
            InitializeComponent();
            MouseDoubleClick += InfoChannelViewer_MouseDoubleClick;
        }

        private void InfoChannelViewer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var viewModel = DataContext as ViewModel.InfoChannelViewerViewModel;
            if( viewModel != null)
            {
                var item = viewModel.SelectedItem;
                if( item != null)
                {
                    ShowDetails(item);
                }
            }
        }

        void ShowDetails( ViewModel.InfoItemModel selectedItem )
        {
            var dlg = new ShowInfoItemDlg(string.Format( "Message from {0}", selectedItem.TimeStamp), selectedItem.Message, selectedItem.Level);
            dlg.ShowDialog();
        }

    }
}
