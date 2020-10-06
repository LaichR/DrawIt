using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;

namespace UI.Utilities.Configuration
{
    public class ConfigurationContainer
    {

        readonly object _wrapped;
        readonly string _name;
        readonly string _fileName;
        readonly string _path;
        readonly List<PropertyInfo> _configurationProperties;
        readonly Dictionary<string, PropertyInfo> _propertyDictionary = new Dictionary<string,PropertyInfo>();

        public ConfigurationContainer( object wrapped )
        {
            _wrapped = wrapped;
            _name = ((ConfigurableEntityAttribute)_wrapped.GetType().GetCustomAttribute(typeof(ConfigurableEntityAttribute))).Name;
            _fileName = string.Format( "{0}.xml", _name );
            _path = UI.Utilities.Environment.PredefinedFolders.GetAndEnsureLocalPath("UiConfiguration");

            _configurationProperties = new List<PropertyInfo>(
                _wrapped.GetType().GetProperties().Where((pi) => pi.GetCustomAttribute(typeof(ConfigurablePropertyAttribute)) != null).
                    OrderBy<PropertyInfo, int>(
                        (pi) => ((ConfigurablePropertyAttribute)pi.GetCustomAttribute(typeof(ConfigurablePropertyAttribute))).Order ));
            foreach( var pi in _configurationProperties)
            {
                _propertyDictionary.Add( pi.Name, pi);
            }
                       
        }

        public void SaveConfig()
        {
            using (var writer = new XmlTextWriter(System.IO.Path.Combine(_path, _fileName), Encoding.ASCII))
            {
                writer.Formatting = Formatting.Indented;
                WriteValuesToXml(writer);
            }
        }

        public void LoadConfig()
        {
            var fullFile = System.IO.Path.Combine(_path, _fileName);
            if (System.IO.File.Exists(fullFile))
            {
                using (var reader = new XmlTextReader(fullFile))
                {
                    var configValues = ReadValuesFromXml(reader);

                    foreach (var pi in _configurationProperties)
                    {
                        
                        if (configValues.TryGetValue(pi.Name, out object value))
                        {
                            ActionDecorator.DecorateCatchAndShowException(string.Format("Error by applying the configuration of: {0}. Property: {1}",
                            _fileName, pi.Name), () => { pi.SetValue(_wrapped, value); })();
                        }
                    }
                }
            }
        }

        void WriteValuesToXml( XmlWriter writer )
        {
            writer.WriteStartDocument();
            writer.WriteStartElement(_name);
            foreach( var pi in this._configurationProperties)
            {
                var value = pi.GetValue(_wrapped);
                
                if (value == null) continue;
                
                writer.WriteStartElement(pi.Name);
                
                if( typeof(IXmlSerializable).IsInstanceOfType(value))
                {
                    var serializable = value as IXmlSerializable;
                    serializable.WriteXml(writer);
                }
                else
                {
                    writer.WriteValue(pi.GetValue(_wrapped));
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        Dictionary<string, object> ReadValuesFromXml(XmlReader reader)
        {
            Dictionary<string, object> propertyValues = new Dictionary<string, object>();
            try
            {
                reader.ReadStartElement(_name);
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (_propertyDictionary.TryGetValue(reader.Name, out PropertyInfo pi))
                        {
                            object value = null;
                            if (pi.PropertyType == typeof(string))
                            {
                                value = reader.ReadElementContentAs(pi.PropertyType, null);
                            }
                            else
                            {
                                value = Activator.CreateInstance(pi.PropertyType);
                                if (typeof(IXmlSerializable).IsInstanceOfType(value))
                                {
                                    var serializable = value as IXmlSerializable;

                                    //Show only the subtree to the serializable object
                                    using (var localReader = reader.ReadSubtree())
                                    {
                                        localReader.ReadToFollowing(reader.Name);
                                        serializable.ReadXml(localReader);
                                    }
                                    value = serializable;
                                }
                                else
                                {
                                    value = reader.ReadElementContentAs(pi.PropertyType, null);
                                }
                            }
                            propertyValues.Add(pi.Name, value);
                        }
                        //reader.ReadEndElement();
                    }
                }
            }
            catch { }
            return propertyValues;
        }
    }
}
