using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Sketch.Helper.UiUtilities;
using Sketch.Helper.Binding;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Sketch.Helper;
using Sketch.Models;
using Sketch.Interface;
using System.Runtime.Serialization;

namespace Sketch.Models.BasicItems
{
    [Serializable]
    public class PictureModel: ConnectableBase
    {
        new public const double DefaultHeight = 100;
        new public const double DefaultWidth = 150;

       
        
        BitmapImage _myImage;
        Brush _fill;

        public PictureModel( System.Windows.Point p, ISketchItemContainer container)
            :base(p, container, new Size(PictureModel.DefaultWidth, PictureModel.DefaultHeight),
                 "new image" , Colors.Snow)
        {
            CanChangeSize = true;
            UpdateGeometry();
        }

        protected PictureModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            UpdateGeometry();
        }

        public override System.Windows.Media.Brush Fill
        {
            get
            {
                return _fill;
            }
        }


        public override void RenderAdornments(DrawingContext drawingContext)
        {
            //base.RenderAdornments(drawingContext);
        }

        protected override void Initialize()
        {
            base.Initialize();
            if (System.IO.File.Exists(Label))
            {
                _myImage = new BitmapImage(new Uri(Label));

                _fill = new ImageBrush(_myImage) { Stretch = Stretch.Fill };
            }
            else
            {
                OpenImage();
            }
        }

        void OpenImage()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "(*.bmp, *.jpg)|*.bmp;*.jpg",
                Title = "Select image"
            };

            if( dlg.ShowDialog() == true )
            {
                Label = dlg.FileName;
                _myImage = new BitmapImage(new Uri(Label));
                _myImage.Rotation = System.Windows.Media.Imaging.Rotation.Rotate90;
                _fill = new ImageBrush(_myImage) { Stretch = Stretch.Fill };
                
            }

        }

    }
}
