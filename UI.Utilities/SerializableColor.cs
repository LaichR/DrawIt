using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Mvvm;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using System.Windows.Media;


namespace UI.Utilities
{
    [Serializable]
    public class SerializableColor : BindableBase, IXmlSerializable, ISerializable
    {

        Color _color;
        Brush _brush;

        public SerializableColor() { }

        protected SerializableColor(SerializationInfo info, StreamingContext context)
        {
            try
            {
                var a = (float)info.GetValue("A", typeof(float));
                var r = (float)info.GetValue("R", typeof(float));
                var g = (float)info.GetValue("G", typeof(float));
                var b = (float)info.GetValue("B", typeof(float));
                Color = new Color() { ScA = a, ScB = b, ScG = g, ScR = r };
            }
            catch { }
            try
            {
                ScRgb = (float[])info.GetValue("Fill", typeof(float[]));
            }
            catch { }
        }

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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Fill", ScRgb, typeof(float[])) ;
            //info.AddValue("A", _color.ScA);
            //info.AddValue("R", _color.ScR);
            //info.AddValue("G", _color.ScG);
            //info.AddValue("B", _color.ScB);
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
