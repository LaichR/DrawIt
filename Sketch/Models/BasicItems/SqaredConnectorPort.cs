using Sketch.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Sketch.Models.BasicItems
{
    [Serializable]
    public class SqaredConnectorPort : DecoratorModel
    {
        readonly static double WidhtAndHeight = 8; 
        Geometry _geometry = new RectangleGeometry(new System.Windows.Rect(0, 0, 15, 15));
        Point _location;
        Rect _bounds = new Rect(0,0, WidhtAndHeight, WidhtAndHeight);

        public SqaredConnectorPort(ConnectorDocking side, ulong id ) :base(side, id){}

        public override Point Location
        {
            get => _location;
            set
            {
                _location = value;
                _bounds.Location = _location;
                _geometry = new RectangleGeometry(_bounds);
            }
        }

        public override Rect Bounds => _bounds;

        SqaredConnectorPort(SerializationInfo info, StreamingContext context):base(info, context) { }

        public override Geometry Geometry => _geometry;
    }
}
