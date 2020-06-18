using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using System.IO;
using Prism.Events;
using Microsoft.Practices.Prism.Mvvm;
using Bluebottle.Base.Interfaces;

namespace Bluebottle.Base.Controls.FolderBrowser.ViewModel
{
    public class BrowserViewModel : BindableBase
    {
        private string _selectedFolder;
        private string _initialPath;

        string _title = "Folder browser";
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                SetProperty<string>(ref _title, value);
            }
        }

        public string SelectedFolder
        {
            get
            {
                return _selectedFolder;
            }
            set
            {
                SetProperty<string>(ref _selectedFolder, value);
            }
        }
        
        public ObservableCollection<FolderViewModel> Folders
        {
            get;
            set;
        }

        public DelegateCommand<object> SpecialFolderSelectedCommand
        {
            get
            {
                return new DelegateCommand<object>(it => SelectedFolder = Environment.GetFolderPath((Environment.SpecialFolder)it));
            }
        }

        public DelegateCommand DownloadFolderSelectedCommand
        {
            get
            {
                return new DelegateCommand(() => SelectedFolder = System.IO.Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Downloads"));
            }
        }

        public DelegateCommand InitialFolderSelectedCommand
        {
            get
            {
                return new DelegateCommand(() => { if (!string.IsNullOrEmpty(_initialPath)) SelectedFolder = _initialPath; });
            }
        }

        public DelegateCommand<object> SelectedFolderEnterPressCommand
        {
            get
            {
                return new DelegateCommand<object>((x) => SelectedFolder = x as string);
            }
        }
        
        public BrowserViewModel(string initialPath)
        {
            _initialPath = initialPath;

            Folders = new ObservableCollection<FolderViewModel>();
            //Environment.GetLogicalDrives().ToList().ForEach(it => Folders.Add(new FolderViewModel(null) { FolderPath = it.TrimEnd('\\'), FolderName = it.TrimEnd('\\'), FolderIcon = Bluebottle.Base.Properties.Resources.HardDisk }));

            foreach (var d in DriveInfo.GetDrives())
            {
                if (d.DriveType == DriveType.Fixed ||
                    d.DriveType == DriveType.Network ||
                    d.DriveType == DriveType.Ram ||
                    d.DriveType == DriveType.Removable)
                {
                    Folders.Add(new FolderViewModel(null)
                    {
                        FolderPath = d.RootDirectory.FullName.TrimEnd('\\'),
                        FolderName = d.Name.TrimEnd('\\'),
                        FolderIcon = Bluebottle.Base.Properties.Resources.HardDisk
                    });
                }
            }
        }
    }
}
