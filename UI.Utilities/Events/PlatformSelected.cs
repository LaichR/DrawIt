using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace Bluebottle.Base.Events
{
    public class PlatformSelected : PubSubEvent<PlatformSelected.Args>
    {
        public class Args
        {
            public string NewPlatformSelection
            {
                get;
                set;
            }

            public string OldPlatformSelection
            {
                get;
                set;
            }
        }

        public PlatformSelected() : base() { }
    }
}
