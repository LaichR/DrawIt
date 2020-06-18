using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;

namespace Bluebottle.Base.Controls.SelectFolder.ViewModel
{
    public class FolderViewModel: BindableBase, Interfaces.IHierarchicalNode
    {
        SelectFolderViewModel _parent;
        FolderViewModel _parentFolder;
        string _name;
        string _fullName;
        bool _isSelected;
        bool _isExpanded;
        bool _isRoot;

        List<FolderViewModel> _subfolders;

        public FolderViewModel(SelectFolderViewModel parent, FolderViewModel parentFolder, string folderName, bool isRoot = false)
        {
            _parentFolder = parentFolder;
            _parent = parent;
            _fullName = folderName;
            _isRoot = isRoot;
            _name = folderName;
            if (!isRoot)
            {
                _name = folderName.Split(System.IO.Path.DirectorySeparatorChar).Last();
            }
            
        }

        public string FullName
        {
            get
            {
                return _fullName;
            }

        }

        public Interfaces.IHierarchicalNode Parent
        {
            get
            {
                return _parentFolder;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public System.Drawing.Bitmap ImageBitmap
        {
            get
            {
                if( _isExpanded)
                {
                    return Bluebottle.Base.Properties.Resources.Folder_Open_icon;
                }
                return Bluebottle.Base.Properties.Resources.Folder_icon;
            }
        }

        public DelegateCommand<string> CmdSetNewRoot
        {
            get
            {
                return _parent.CmdSetNewRoot;
            }
        }

        public List<FolderViewModel> Subfolders
        {
            get { 
                if( _subfolders == null)
                {
                    _subfolders = new List<FolderViewModel>(
                        System.IO.Directory.EnumerateDirectories(FullName).
                            Select<string, FolderViewModel>((x)=> new FolderViewModel(_parent, this, x)));
                }
                return _subfolders; }
        }

        public bool IsSelected
        {
            get { 
                return _isSelected; }
            set { 
                SetProperty<bool>(ref _isSelected, value);
                _parent.SelectedItem = this;
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                SetProperty<bool>(ref _isExpanded, value);
                OnPropertyChanged("ImageBitmap");
            }
        }

        public bool EditModeOn
        {
            get;
            set;
        }
    }
}
