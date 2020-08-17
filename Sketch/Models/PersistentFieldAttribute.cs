using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Sketch.Models
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PersistentFieldAttribute:Attribute
    {
        public PersistentFieldAttribute(ModelVersion availableSinceVersion, 
            string serialisationName, bool hasDefault = false)
        {
            HasDefault = hasDefault;
            AvalailableSince = availableSinceVersion;
            Name = serialisationName;
        }

        public string Name
        {
            get;
            private set;
        }

        public bool HasDefault
        {
            get;
            private set;
        }

        public ModelVersion AvalailableSince
        {
            get;
            private set;
        }

    }
}
