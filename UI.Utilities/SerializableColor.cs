using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Mvvm;
using System.Xml.Serialization;

using System.Windows.Media;


namespace UI.Utilities
{
    public class SerializableColor : BindableBase, IXmlSerializable
    {

        Color _color;
        Brush _brush;

        public Color Color
        {
            get { return _color; }
            set { 
                SetProperty<Color>(ref _color, value);
                _brush = new SolidColorBrush(Color);
            }
        }

        public Brush Brush
        {
            get
            {
                return _brush;
            }
        }

        public float[] ScRgb
        {
            get
            {
                List<float> scrgb = new List<float>();
                scrgb.Add(_color.ScA );
                scrgb.Add(_color.ScR);
                scrgb.Add(_color.ScG);
                scrgb.Add(_color.ScB);
                return scrgb.ToArray();
            }
            set
            {
                Color = Color.FromScRgb(value[0], value[1], value[2], value[3]);
                
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            var color = reader.ReadString();
            var values = color.Split(',');
            List<float> valueList = new List<float>();
            foreach( var v in values)
            {
                valueList.Add( float.Parse(v) );
            }
            ScRgb = valueList.ToArray();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            var values = ScRgb;
            writer.WriteString(string.Format("{0},{1},{2},{3}", values ));
        }
    }
}
