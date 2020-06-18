using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Utilities.Configuration
{
    public class ConfigurableEntityAttribute: Attribute
    {
        public ConfigurableEntityAttribute( string name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            private set;
        }
    }
}
