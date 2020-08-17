using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sketch.Models
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple =false)]
    public class DoNotConsiderForConnectorRoutingAttribute: Attribute{}
}
