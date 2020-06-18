using Bluebottle.Base.Controls.FindAndReplaceDialogBox.ViewModel;
using Bluebottle.Base.Interfaces;
using Bluebottle.Base.Events;
using Prism.Events;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Bluebottle.Base.Controls.FindAndReplaceDialogBox.View
{
    /// <summary>
    /// Interaction logic for FindAndReplace.xaml
    /// </summary>
    public partial class FindAndReplace : Window
    {
        IEventAggregator _eventAggregator;
        bool _appIsClosing;
        FindAndReplaceViewModel _findReplaceViewModel { get; set; }
        FindAndReplaceOperation _operation;

        private void CreateNewModel(Scintilla editor, List<ITextAccessor> editors, FindAndReplaceOperation operation, string location) 
        {
            if( _findReplaceViewModel != null )
            {
                _findReplaceViewModel.Release();
            }
            _findReplaceViewModel = new FindAndReplaceViewModel(editor, editors, operation, location);
            DataContext = _findReplaceViewModel;
        }

        public FindAndReplace(IEventAggregator eventAggregator,
                              ScintillaNET.Scintilla activeEditor,
                              List<ITextAccessor> getTexts,
                              FindAndReplaceOperation operation,
                              string location)
        {
            InitializeComponent();
            _operation = operation;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AppIsClosing>().Subscribe(OnAppIsClosing);
            
            CreateNewModel(activeEditor, getTexts, operation, location);
        }

        public void Update(ScintillaNET.Scintilla activeEditor,
                           List<ITextAccessor> getTexts,
                           FindAndReplaceOperation operation,
                           string location)
        {
            _operation = operation;
            CreateNewModel(activeEditor, getTexts, operation, location);
        }

        public ScintillaNET.Scintilla ActiveEditor
        {
            get { return _findReplaceViewModel.SearchEditor; }
            set { _findReplaceViewModel.SearchEditor = value; }
        }

        public void RegisterOnSearchEditorChange(EventHandler<int> onSearchEditorChange )
        {
            _findReplaceViewModel.SearchEditorChange += onSearchEditorChange;
        }
        

        public void Display()
        {
            this.Show();
            this.Activate();
        }

        protected override void OnDeactivated(EventArgs e)
        {
            _findReplaceViewModel.SaveConfiguration();
            base.OnDeactivated(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_appIsClosing)
                base.OnClosing(e);
            else
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        void OnAppIsClosing(AppIsClosing.Args args)
        {
            _appIsClosing = true;
            this.Close();
        }

        private void OnKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }
    }
}
