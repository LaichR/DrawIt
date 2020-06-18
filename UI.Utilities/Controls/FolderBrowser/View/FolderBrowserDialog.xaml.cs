using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Bluebottle.Base.Controls.FolderBrowser.ViewModel;

namespace Bluebottle.Base.Controls.FolderBrowser.View
{
    /// <summary>
    /// Interaction logic for FolderBrowserDialog.xaml
    /// </summary>
    public partial class FolderBrowserDialog : Window
    {
        private BrowserViewModel _viewModel;
        private string _initialPath;

        public BrowserViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        public string SelectedFolder
        {
            get
            {
                return ViewModel.SelectedFolder;
            }
        }

        public bool IsSelectionOk
        {
            get
            {
                return DialogResult.HasValue && DialogResult.Value;
            }
        }

        public FolderBrowserDialog(string title, string initialPath)
        {
            InitializeComponent();
            _viewModel = new BrowserViewModel(initialPath);
            FolderTree.DataContext = _viewModel;
            FolderTree.PathDelimiter = '\\'; // System.IO.Path.PathSeparator;
            DataContext = _viewModel;
            _viewModel.Title = title;
            _initialPath = initialPath;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void FolderTree_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_initialPath) && System.IO.Directory.Exists(_initialPath))
            {
                FolderTree.SelectedPath = _initialPath;
            }
        }
    }
}
