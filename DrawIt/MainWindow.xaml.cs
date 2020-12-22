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


using Sketch;
using Sketch.Controls;
using Sketch.Interface;
using Sketch.Controls.ColorPicker;
using Sketch.Models;

namespace DrawIt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        readonly ApplicationViewModel _model;
        Sketch.Controls.ColorPicker.ColorPalette _palette;
        TreeViewItem _focusedItem;

        public MainWindow()
        {
            InitializeComponent();


            _model = new ApplicationViewModel(); 
            DataContext = _model;

            this.SketchPad.DataContext = DataContext;
            //this.OutlineView.DataContext = DataContext;
            
            InitTools(_model.FileTools);
            //InitTools(_model.Sketch.SketchItemFactory.Palette);
            InitTools(_model.AlignmentTools);
            InitTools(_model.ZoomTools);
            InitColorTools();
            
            
        }


        void InitColorTools()
        {
            var tb = new ToolBar();
            tb.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            
            _palette = new Sketch.Controls.ColorPicker.ColorPalette();
            _palette.ConnectModel(_model.Sketch);
            tb.Items.Add(_palette);
            
            ToolbarPanel.Children.Add(tb);
        }

       

        void InitTools(IEnumerable<UI.Utilities.Interfaces.ICommandDescriptor> commands)
        {
            var tb = new ToolBar();
            tb.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            foreach (var cm in commands)
            {
                tb.Items.Add(new UI.Utilities.Controls.ToolbarButton { Command = cm.Command, ImageBitmap = cm.Bitmap, ToolTip = cm.Name });
            }
            ToolbarPanel.Children.Add(tb);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void SketchPad_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            var selected = SketchPad.SelectedItem;
            _model.SelectedItem = selected;
            PropertyEditor.InspectedObject = selected;
        }


        private void TreeView_LostFocus(object sender, RoutedEventArgs e)
        {
            if( e.OriginalSource is TreeViewItem item 
                && e.Source is Sketch.Models.SketchItemFactory)
            {
                item.IsExpanded = false;
            }
        }

        private void TreeView_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TreeViewItem item && item.Header is 
                Sketch.Models.SketchItemGroup)
            {
                if( _focusedItem != null)
                {
                    _focusedItem.IsExpanded = false;
                }
                item.IsExpanded = true;
                _focusedItem = item;
            }
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            var col = MainGrid.ColumnDefinitions[0];
            col.Width = new GridLength(2, GridUnitType.Star);
            MainGrid.ColumnDefinitions[1].Width = new GridLength(5, GridUnitType.Pixel);
            MainGrid.ColumnDefinitions[2].Width = new GridLength(5, GridUnitType.Star);
            
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            MainGrid.ColumnDefinitions[0].Width = new GridLength(25, GridUnitType.Pixel);
            MainGrid.ColumnDefinitions[2].Width = new GridLength(10, GridUnitType.Star);
        }

    }
}
