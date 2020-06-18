using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Windows;
using System.Xml.Serialization;

namespace Bluebottle.Base
{
    public class SerializableGridLength : BindableBase, IXmlSerializable
    {

        GridLengthConverter _converter;
        GridLength _gridLength;

        public SerializableGridLength()
        {
            _converter = new GridLengthConverter();
            _gridLength = new GridLength(1, GridUnitType.Star);
        }


        public GridLength GridLength
        {
            get { return _gridLength; }
            set { SetProperty<GridLength>(ref _gridLength, value); }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            _gridLength = (GridLength)_converter.ConvertFromInvariantString(reader.ReadString());
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteString(_converter.ConvertToInvariantString(_gridLength));

        }

    }
}
