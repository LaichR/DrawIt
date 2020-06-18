using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Mvvm;
using Bluebottle.Base.Interfaces;

namespace Bluebottle.Base.Controls.FolderBrowser.ViewModel
{
    public class FolderViewModel : BindableBase, Bluebottle.Base.Interfaces.IHierarchicalNode
    {
        private bool _isSelected;
        private bool _isExpanded;
        private System.Drawing.Bitmap _folderIcon;
        IHierarchicalNode _parent;


        public System.Drawing.Bitmap FolderIcon
        {
            get
            {
                return _folderIcon;
            }
            set
            {
                _folderIcon = value;
                OnPropertyChanged("FolderIcon");
            }
        }

        public string FolderName
        {
            get;
            set;
        }

        public string FolderPath
        {
            get;
            set;
        }

        public ObservableCollection<FolderViewModel> Folders
        {
            get;
            set;
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;

                    //SetProperty<bool>(ref _isExpanded, value);
                    OnPropertyChanged("IsExpanded");

                    if (!FolderName.Contains(':'))//Folder icon change not applicable for drive(s)
                    {
                        if (_isExpanded)
                            FolderIcon = Bluebottle.Base.Properties.Resources.Folder_Open_icon;
                        else
                            FolderIcon = Bluebottle.Base.Properties.Resources.Folder_icon;
                    }

                    LoadFolders();
                }
            }
        }

        public IHierarchicalNode Parent { get { return _parent; } }

        public string Name
        {
            get
            {
                return FolderName;
            }
        }

        public bool EditModeOn { get; set; }

        private void LoadFolders()
        {
            try
            {
                if (Folders.Count > 0)
                    return;

                string[] dirs = null;

                string fullPath = Path.Combine(FolderPath, FolderName);

                if (FolderName.Contains(':'))//This is a drive
                    fullPath = string.Concat(FolderName, "\\");
                else
                    fullPath = FolderPath;

                dirs = Directory.GetDirectories(fullPath);

                Folders.Clear();

                foreach (string dir in dirs)
                    Folders.Add(new FolderViewModel(this) { FolderName = Path.GetFileName(dir), FolderPath = Path.GetFullPath(dir), FolderIcon = Bluebottle.Base.Properties.Resources.Folder_icon});

                if (FolderName.Contains(":"))
                    FolderIcon = Bluebottle.Base.Properties.Resources.HardDisk;
            }
            catch (UnauthorizedAccessException ae)
            {
                Console.WriteLine(ae.Message);
            }
            catch (IOException ie)
            {
                Console.WriteLine(ie.Message);
            }
        }

        public FolderViewModel(IHierarchicalNode parent)
        {
            _parent = parent;
            Folders = new ObservableCollection<FolderViewModel>();
        }
    }
}
