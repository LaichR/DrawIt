using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using System.Windows.Media;


namespace UI.Utilities
{
    [Serializable]
    public class SerializableColor : BindableModel, IXmlSerializable, ISerializable, IConvertible
    {

        Color _color;
        Brush _brush;

        public SerializableColor() { }

        protected SerializableColor(SerializationInfo info, StreamingContext context)
        {
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
                List<float> scrgb = new List<float>()
                {
                    _color.ScA,
                    _color.ScR,
                    _color.ScG,
                    _color.ScB
                };
                
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
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public static implicit operator SerializableColor(float[] scrgb)
        {
            return new SerializableColor() { ScRgb = scrgb };
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
            writer.WriteString(string.Format("{0},{1},{2},{3}", values[0], values[1], values[2], values[3]  ));
        }

        TypeCode IConvertible.GetTypeCode()
        {
            throw new NotImplementedException();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}
