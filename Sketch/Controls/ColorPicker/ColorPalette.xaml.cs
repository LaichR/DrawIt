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

namespace Sketch.Controls.ColorPicker
{
    /// <summary>
    /// Interaction logic for ColorPalette.xaml
    /// </summary>
    public partial class ColorPalette : UserControl
    {
        public ColorPalette()
        {
            InitializeComponent();
            Height = 24;
        }

        public void ConnectModel( IColorSelectionTarget target)
        {
            DataContext = new ColorPaletteModel( target );
        }
    }
}
