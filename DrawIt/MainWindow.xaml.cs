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
        ISketchItemFactory _sketchItemFactory;
        ApplicationViewModel _model;
        Sketch.Controls.ColorPicker.ColorPalette _palette;


        public MainWindow()
        {
            InitializeComponent();


            _sketchItemFactory = new Uml.UmlShapeFactory();
            Sketch.ModelFactoryRegistry.Instance.PushSketchItemFactory(_sketchItemFactory);

            _sketchItemFactory.RegisterBoundedItemSelectedNotification( factory_OnSelectElement );
            _sketchItemFactory.RegisterConnectorItemSelectedNotification( this.SketchPad.HandleAddConnector );

            _model = new ApplicationViewModel(
                (x) => SketchPad.SavePictureAs(x),
                () => SketchPad.OpenChildElement(),
                () => SketchPad.CloseChildElement()
            ); 
            DataContext = _model;

            this.SketchPad.DataContext = DataContext;
            this.OutlineView.DataContext = DataContext;
            
            InitTools(_model.FileTools);
            InitTools(_sketchItemFactory.Palette);
            InitTools(_model.AlignmentTools);
            InitTools(_model.ZoomTools);
            InitColorTools();
            
            
        }

        void factory_OnSelectElement(object sender, EventArgs e)
        {
            _model.IsInsertMode = true;
            
        }

        void InitColorTools()
        {
            var tb = new ToolBar();
            tb.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            
            _palette = new Sketch.Controls.ColorPicker.ColorPalette();
            _palette.ConnectModel(_model);
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

    }
}
