using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;

namespace Bluebottle.Base.Controls.SelectFolder.ViewModel
{
    public class SelectFolderViewModel :BindableBase
    {

        DelegateCommand _ok;
        DelegateCommand _cancel;
        DelegateCommand _gotoParent;
        DelegateCommand<string> _setNewRoot;

        bool _isSelectionOk = false;
        bool _isProcessing = false;
        string _title;
        string _rootPath;
        FolderViewModel _selectedItem;
        string _selectedPath;
        List<FolderViewModel> _folderList;

        public SelectFolderViewModel(String title, string root, Action closeWindow)
        {
            _title = title;
            if( !System.IO.Directory.Exists(root))
            {
                throw new ArgumentException("Invalid path specified");
            }

            _ok = new DelegateCommand(() => { _isSelectionOk = _selectedPath != null; closeWindow(); });
            _cancel = new DelegateCommand(() => { _isSelectionOk = false; closeWindow(); });
            _gotoParent = new DelegateCommand(actionGotoParent);
            _setNewRoot = new DelegateCommand<string>(actionSetNewRoot );
            RootPath = root;
        }

        public string Title
        {
            get { return _title; }
        }

        public string RootPath
        {
            get
            {
                return _rootPath;
            }
            set
            {
                SetProperty<string>(ref _rootPath, value);
                _folderList = new List<FolderViewModel>();
                _folderList.Add(new FolderViewModel(this,null, _rootPath, true));
                _folderList[0].IsSelected = true;
                _folderList[0].IsExpanded = true;
                OnPropertyChanged("FolderList");
            }
        }

        public IEnumerable<FolderViewModel> FolderList
        {
            get
            {
                return _folderList;
            }
        }

        public System.Drawing.Bitmap BmpGoToParent
        {
            get
            {
                return Bluebottle.Base.Properties.Resources.Open_file_icon;
            }
        }

        public FolderViewModel SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty<FolderViewModel>(ref _selectedItem, value);
            SelectedPath = _selectedItem.FullName;
            }
        }

        public bool IsSelectionOk
        {
            get
            {
                return _selectedItem != null && _isSelectionOk;
            }
        }

        public string SelectedPath
        {
            get
            {
                return _selectedPath;
            }
            set
            {
                SetProperty<string>(ref _selectedPath, value);
            }
        }

        public InitialsSearchDelegate SearchInitials
        {
            get
            {
                return (x, y) => ((FolderViewModel)x).Name.StartsWith(y, StringComparison.CurrentCultureIgnoreCase);
            }
        }

        public DelegateCommand CmdOkClose
        {
            get
            {
                return _ok;
            }
        }

        public DelegateCommand CmdCancelClose
        {
            get
            {
                return _cancel;
            }
        }

        public DelegateCommand CmdGotoParent
        {
            get
            {
                return _gotoParent;
            }
        }

        public DelegateCommand<string> CmdSetNewRoot
        {
            get
            {
                return _setNewRoot;
            }
        }



        private void actionGotoParent()
        {
            var root = RootPath;
            var dirInfo = System.IO.Directory.GetParent(root);
            if (dirInfo != null)
            {
                var newRoot = dirInfo.FullName;
                RootPath = newRoot;
            }
        }

        private void actionSetNewRoot(string newRoot)
        {
            _isProcessing = true;
            // since the double click comes more than once, there needs to be a protection
            // otherwise we will always reset the value to the previos folder!?
            if( System.IO.Directory.Exists(newRoot) && newRoot == _selectedItem.FullName )
            {
                RootPath = newRoot;
            }
            _isProcessing = false;
        }


    }


}
