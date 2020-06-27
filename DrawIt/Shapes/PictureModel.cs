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
        IList<UI.Utilities.Interfaces.ICommandDescriptor> _tools;
        OutlineToolFactory _toolFactory;
        BitmapImage _myImage;
        Brush _fill;

        public PictureModel( System.Windows.Point p, string fileName, OutlineToolFactory toolFactory)
        {
            Label = fileName;
            InitImageBrush();
            Bounds = new Rect(p, new System.Windows.Size(_myImage.Width, _myImage.Height));
            IsSelected = true;
            AllowSizeChange = true;
            RotationAngle = 0.0;
            FillColor = System.Windows.Media.Colors.Snow;
            _toolFactory = toolFactory;
            _tools = _toolFactory.GetTools();   
        }

        protected PictureModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _toolFactory = (OutlineToolFactory)
            info.GetValue("ToolFactory", typeof(OutlineToolFactory));
            _tools = _toolFactory.GetTools();
            InitImageBrush();
        }

        public override System.Windows.Media.Brush Fill
        {
            get
            {
                return _fill;
            }
        }

        public override System.Windows.Media.RectangleGeometry Outline
        {
            get { return new RectangleGeometry(Bounds); }
        }

        //public override IList<UI.Utilities.Interfaces.ICommandDescriptor> Tools
        //{
        //    get { return _tools; }
        //}

        public override System.Windows.Media.Geometry Geometry
        {
            get { return new RectangleGeometry(Bounds); }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ToolFactory", _toolFactory);
            
        }

        public override void RenderAdornments(DrawingContext drawingContext)
        {
            //base.RenderAdornments(drawingContext);
        }

        private void InitImageBrush()
        {
            _myImage = new BitmapImage(new Uri(Label));
            _fill = new ImageBrush() { ImageSource = _myImage };
        }
    }
}
