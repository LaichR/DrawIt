using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ScintillaNET;
using Prism.Mvvm;
using System.Windows.Input;
using Prism.Commands;
using System.Windows.Media;
using Bluebottle.Base.Configuration;
using Bluebottle.Base.Interfaces;
using System.Windows;

namespace Bluebottle.Base.Controls.FindAndReplaceDialogBox.ViewModel
{
    [ConfigurableEntity("PersistentFindReplaceSettings")]
    public class FindAndReplaceViewModel: BindableBase
    {
        #region Class private members, constructor and destructor

        List<ITextAccessor> _texts;    // all opened Scintilla controls
        Scintilla _editor;           // current Scintilla control
        int _editorIndex;   
        SearchFlags _searchFlags;    // Search Flags for Scintilla editor

        // search settings
        SearchSettings _sfSettings;  // search flags
        SearchSettings _sdSettings;  // search direction
        SearchSettings _ssSettings;  // search scope
        bool _wrapAroundIsChecked;
        bool _wrapAroundIsEnabled;
        bool _findOnAssign;


        Regex _findExpression = null;
        string _findText = "";
        string _replaceText = "";

        // window title, icon, position
        string _location;
        string _tabTitle;   
        ImageSource _icon;
        int _findReplaceWindowLeft = 10;
        int _findReplaceWindowTop = 10;
        ConfigurationContainer _propertyConfig;
        int _selectedTabIndex;
        int _processedEditors = 0;
        int _initialEditor = 0;
        int _searchPosition = 0;

        Boolean _calledReplaceAll = false;

        public FindAndReplaceViewModel(Scintilla editor, List<ITextAccessor> texts, FindAndReplaceOperation operation, string location) 
        {
            
            _texts = texts;
            var textList = new List<string>(_texts.Select((x) => x.GetText()));
            _editorIndex = textList.IndexOf(editor.Text);
            _initialEditor = _editorIndex;
            _searchFlags = new SearchFlags();

            SelectedTabIndex = operation == FindAndReplaceOperation.Find? 0 : 1;
            _location = location;
            _tabTitle = operation.ToString();
            
            // searchflags
            var sf = new List<string>() { "Whole Word", "Match Case", "Word Start", "Regex" };
            _sfSettings = new SearchSettings("Search Settings", sf);

            //search direction & default is set to Down
            var sd = new List<string>() { "Down", "Up"};
            _sdSettings = new SearchSettings("Search Direction", sd);
            _sdSettings.Children[0].IsChecked = true;

            //search scope & default is set to Current File
            var ss = new List<string>() { "Current File", "All Opened Files"};
            _ssSettings = new SearchSettings("Search Scope", ss);
            _ssSettings.Children[0].IsChecked = true;

            _wrapAroundIsChecked = true;
            _wrapAroundIsEnabled = true;
            

            _icon = ToBitmapSource.Bitmap2BitmapSource(Bluebottle.Base.Properties.Resources.find);
            _propertyConfig = new ConfigurationContainer(this);
            _propertyConfig.LoadConfig();

            _findOnAssign = false;
            SearchEditor = editor;
            
            if (!String.IsNullOrEmpty(SearchEditor.SelectedText))
            {
                FindText = SearchEditor.SelectedText;
            }
            else
            {
                ResetCurrentPosition();
            }
            _searchPosition = editor.CurrentPosition;
        }

        #endregion

        #region Properties bound to View

        public ImageSource FindAndReplaceIcon
        {
            get { return _icon; }
        }

        [ConfigurableProperty(0)]
        public int FindReplaceWindowLeft
        {
            get { return _findReplaceWindowLeft; }
            set { SetProperty<int>(ref _findReplaceWindowLeft, value); }
        }

        [ConfigurableProperty(1)]
        public int FindReplaceWindowTop
        {
            get { return _findReplaceWindowTop; }
            set { SetProperty<int>(ref _findReplaceWindowTop, value); }
        }

        //[ConfigurableProperty(2)]
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                _selectedTabIndex = value;
                if (_selectedTabIndex == 0)
                    _tabTitle = "Find";
                else
                    _tabTitle = "Replace";
                OnPropertyChanged("WindowTitle");
                OnPropertyChanged("SelectedTabIndex");
            }
        }

        public string WindowTitle
        {
            get { return String.Format("{1} in {0}", _location, _tabTitle); }
        }
        
        [ConfigurableProperty(2)] 
        public string FindText
        {
            get { return _findText; }
            set
            {
                _findText = value;
                _findExpression = null;
                OnPropertyChanged("FindText");
            }
        }
        
        [ConfigurableProperty(2)] 
        public string ReplaceText
        {
            get { return _replaceText; }
            set
            {
                _replaceText = value;
                OnPropertyChanged("ReplaceText");
            }
        }

        [ConfigurableProperty(2)]
        public bool WrapAroundCheck
        {
            get { return _wrapAroundIsChecked; }
            set { SetProperty<bool>(ref _wrapAroundIsChecked, value); }
        }

        [ConfigurableProperty(2)]
        public bool WrapAroundEnable
        {
            get { return _wrapAroundIsEnabled; }
            set { SetProperty<bool>(ref _wrapAroundIsEnabled, value); }
        }

        [ConfigurableProperty(1)]
        public SearchSettings SfSettings
        {
            get { return _sfSettings; }
            set
            {
                _sfSettings.SetChildrenState(value.Children);
                OnPropertyChanged("SfSettings");
            }
        }

        [ConfigurableProperty(1)]
        public SearchSettings SdSettings
        {
            get { return _sdSettings; }
            set
            {
                _sdSettings.SetChildrenState(value.Children);
                OnPropertyChanged("SdSettings");
            }
        }

        [ConfigurableProperty(1)]
        public SearchSettings SSSettings
        {
            get { return _ssSettings; }
            set
            {
                _ssSettings.SetChildrenState(value.Children);
                OnPropertyChanged("SSSettings");
            }
        }

        public ICommand CmdFindNext
        {
            get { return new DelegateCommand(() => { FindNext(); }); }
        }

        public ICommand CmdReplace
        {
            get { return new DelegateCommand(() => { Replace(); }); }
        }

        public ICommand CmdReplaceAll
        {
            get { return new DelegateCommand(() => { ReplaceAll(); }); }
        }

        public ICommand CmdSearchScopeChanged
        {
            get { return new DelegateCommand(() => { SearchScopeChanged(); }); }
        }

        public ICommand CmdSearchDirectionChanged
        {
            get { return new DelegateCommand(() => { SearchDirectionChanged(); }); }
        }

        #endregion

        #region Find and Replace functions

        private void FindNext()
        {

            if (GetScope() == "Current File")
            {
                FindInCurrentEditor();
            }
            else
            {
                
                // search in current editor
                FindInCurrentEditor();

                //_processedEditors++;
                if (_searchPosition == -1)
                {
                    if (SwitchToNewEditor())
                    {
                        //_processedEditors--; // in case of a match the increment will be called twice!
                       
                        //switch to new editor, notify gui to display the new selection                    
                        NotifySearchEditorChange(_editorIndex, true);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("You have searched through all files!");
                        ResetCurrentPosition();
                        _processedEditors = 0;
                    }
                }
            }

        }


        private void FindInCurrentEditor()
        {            
            SetSearchFlags();
            SearchEditor.SearchFlags = _searchFlags;
            if( _searchPosition == -1 )
            {
                ResetCurrentPosition();
            }

            _searchPosition = Find();

            if (WrapAroundCheck)
            {
                if (_searchPosition == -1)
                {
                    ResetCurrentPosition();
                    _searchPosition = Find();
                    if (_searchPosition == -1)
                        System.Windows.Forms.MessageBox.Show(String.Format("Can't find text: {0}.", FindText));
                }
            }
            else
            {
                if (_searchPosition == -1 && GetScope() == "Current File")
                {
                    if (_calledReplaceAll)
                        System.Windows.Forms.MessageBox.Show("Replace all finished!");
                    else
                        System.Windows.Forms.MessageBox.Show(String.Format("Can't find text: {0}. Try searching in opposite direction or check Wrap Around option.", FindText));
                }
            }
        }
        
        private int Find() 
        {
            if (GetDirection() == "Down")
            {
                _editor.TargetStart = Math.Max(_editor.CurrentPosition, _editor.AnchorPosition);
                _editor.TargetEnd = _editor.TextLength;
            }
            else if (GetDirection() == "Up")
            {
                _editor.TargetStart = Math.Min(_editor.CurrentPosition, _editor.AnchorPosition);
                _editor.TargetEnd = 0;
            }

            var pos = _editor.SearchInTarget(FindText);
            if (pos >= 0)
            {
                _editor.SetSel(_editor.TargetStart, _editor.TargetEnd);
            }
            else
            {
                if (GetDirection() == "Up")
                    _editor.CurrentPosition = Math.Min(_editor.CurrentPosition, _editor.AnchorPosition);
                else
                    _editor.CurrentPosition = Math.Max(_editor.CurrentPosition, _editor.AnchorPosition);
                _editor.SetEmptySelection(_editor.CurrentPosition); // remove any selection
            }
            return pos;
        }

        private int Replace()
        {
            var pos = -1;
            if (!String.IsNullOrEmpty(_editor.SelectedText))
            {
                // text to replace is already selected
                _editor.ReplaceSelection(_replaceText);
                pos = _editor.CurrentPosition;
            }
            //else
            //{
            //    // text to replace is not yet selected
            //    // find the text and, if found, replace the selection
            //    FindNext();
            //    if (_searchPosition >= 0)
            //        _editor.ReplaceSelection(_replaceText);
            //}
            // ReplaceSelection positions currentPosition and anchorPosition 
            // at the end of replaced text. For searching "Up", this positions should be set
            // at the beginning of replaced text.
            if (GetDirection() == "Up")
            {
                _editor.CurrentPosition = _editor.CurrentPosition - _replaceText.Length - 1;
                _editor.AnchorPosition = _editor.CurrentPosition;
            }
            FindNext();
            return _searchPosition;
        }

        private void ReplaceAll()
        {
            _calledReplaceAll = true;
            ResetCurrentPosition();
            var wrapAroundsetting = _wrapAroundIsChecked;
            _wrapAroundIsChecked = false;
            foreach( var accessor in _texts)
            {
                var t = accessor.GetText();
                accessor.SetText( FindExpression.Replace(t, _replaceText));
            }
           _wrapAroundIsChecked = wrapAroundsetting;
           _calledReplaceAll = false;
        }

        #endregion

        #region Helper functions

        private void SetSearchFlags()
        {
            _searchFlags = SearchFlags.None;
            foreach (var s in _sfSettings.Children)
            {
                if (s.IsChecked)
                {
                    switch (s.Name)
                    {
                        case "Match Case":
                            _searchFlags |= SearchFlags.MatchCase;
                            break;
                        case "Whole Word":
                            _searchFlags |= SearchFlags.WholeWord;
                            break;
                        case "Regex":
                            _searchFlags |= SearchFlags.Regex;
                            break;
                        case "Word Start":
                            _searchFlags |= SearchFlags.WordStart;
                            break;
                        case "Posix":
                            _searchFlags |= SearchFlags.Posix;
                            break;
                    }                    
                }
            }
            _findExpression = null;
        }

        private string GetDirection()
        {
            if (_sdSettings.Children.Any(x => x.IsChecked == true))
            {
                var selectedItem = _sdSettings.Children.Where(x => x.IsChecked == true).First();
                return selectedItem.Name;
            }
            else
            {
                _sdSettings.Children[0].IsChecked = true;
                return GetDirection();
            }
        }

        private string GetScope()
        {
            var selectedItem = _ssSettings.Children.Where(x => x.IsChecked == true).First();
            return selectedItem.Name;
        }


        private void ResetCurrentPosition()
        {
            if (GetDirection() == "Down")
            {
                _editor.CurrentPosition = 0;
                _editor.AnchorPosition = 0;
            }
            else if (GetDirection() == "Up")
            {
                _editor.CurrentPosition = _editor.TextLength;
                _editor.AnchorPosition = _editor.TextLength;
            }
        }

        private void SearchDirectionChanged()
        {
            if (String.IsNullOrEmpty(_editor.SelectedText))
            {
                ResetCurrentPosition();
            }
        }

        private void SearchScopeChanged()
        {
            if (GetScope() == "Current File")
                WrapAroundEnable = true;
            else
            {
                WrapAroundCheck = false;
                WrapAroundEnable = false;
            }
            _initialEditor = 0;
            if (String.IsNullOrEmpty(_editor.SelectedText))
            {
                ResetCurrentPosition();
            }
        }


        private bool SwitchToNewEditor() 
        {
            var nrOfTexts = _texts.Count;
            bool found = false;
            var searchText = "";
            while (!found && _processedEditors < nrOfTexts)
            {
                _processedEditors++;
                if (GetDirection() == "Down")
                {
                    _editorIndex++;
                    if (_editorIndex >= nrOfTexts)
                        _editorIndex = 0;
                }
                else // direction Up 
                {
                    _editorIndex--;
                    if (_editorIndex < 0)
                        _editorIndex = nrOfTexts - 1;
                }
                searchText = _texts[_editorIndex].GetText();
                if (searchText == null) searchText = "";
                found = FindExpression.IsMatch(searchText);
                
            }
            
            return found;
        }

        #endregion

        public event EventHandler<int> SearchEditorChange;

        public void NotifySearchEditorChange(int index, bool findOnAssign)
        {
            _findOnAssign = findOnAssign;
            if (SearchEditorChange != null)
            {
                SearchEditorChange(this, _editorIndex);
            }
        }

        public Scintilla SearchEditor
        {
            get
            {
                return _editor;
            }

            set
            {
                _editor = value;
                if( _findOnAssign )
                {
                    ResetCurrentPosition();
                    _editor.SearchFlags = _searchFlags;
                    _searchPosition = Find();
                }
            }
        }

        internal void SaveConfiguration()
        {
            _propertyConfig.SaveConfig();
        }

        Regex FindExpression
        {
            get
            {
                if( _findExpression == null)
                {
                    var option = RegexOptions.None;

                    if ((SearchFlags.MatchCase & _searchFlags) == 0) //ignore case
                    {
                        option = RegexOptions.IgnoreCase;
                    }

                    var regex = FindText;
                    if( (SearchFlags.Regex&_searchFlags)==0) // do not use regular expression
                    {
                        regex = Regex.Escape(regex);// esacpe any characters that are used by regex
                    }
                    if (((SearchFlags.WordStart|SearchFlags.WholeWord) & _searchFlags) != 0) // use regular expression
                    {
                        if (((SearchFlags.WordStart) & _searchFlags) == _searchFlags)
                        {
                            regex = string.Format(@"\W{0}",regex);
                        }
                        else
                        {
                            regex = string.Format(@"\W{0}\W", regex);
                        }
                    }
                    _findExpression = new Regex(regex, option);
                }
                return _findExpression;
            }
        }

        internal void Release()
        {
            // release all attached handlers; otherwise we will create a
            // memory leak, since the object will never be collected!
            // how about OnPropertyChanged? hopefully these will be released when
            // the data context of the view is reassigned!
            this.SearchEditorChange = null;
        }

    }
}
