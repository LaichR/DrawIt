using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;


namespace Bluebottle.Base.Events
{
    public class AppIsClosing : PubSubEvent<AppIsClosing.Args>
    {
        public class Args
        {
            public Args()
            {
            }
        }

        public AppIsClosing() : base() { }
    }
}
