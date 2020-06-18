using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Utilities.Configuration
{
    public class ConfigurablePropertyAttribute: Attribute
    {
        public ConfigurablePropertyAttribute(int initialisationOrder)
        {
            Order = initialisationOrder;
        }

        public int Order
        {
            get;
            private set;
        }
    }
}
