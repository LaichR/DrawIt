using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Utilities.Interfaces;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Sketch.Types;
using Sketch.Models;
using System.Runtime.Serialization;

namespace DrawIt.Shapes
{
    [Serializable]
    public class PictureModel: ConnectableBase
    {
        new public const double DefaultHeight = 100;
        new public const double DefaultWidth = 150;

       
        
        BitmapImage _myImage;
        Brush _fill;

        public PictureModel( System.Windows.Point p, string fileName)
            :base(p, new Size(PictureModel.DefaultWidth, PictureModel.DefaultHeight),
                 fileName, Colors.Snow)
        {
            Label = fileName;
            AllowSizeChange = true;
            RotationAngle = 0.0;
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
            _myImage = new BitmapImage(new Uri(Label));
            _fill = new ImageBrush(_myImage) { Stretch = Stretch.Fill };
        }

    }
}
