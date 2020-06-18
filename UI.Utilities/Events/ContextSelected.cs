using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace Bluebottle.Base.Events
{

    public class ContextSelected:PubSubEvent<ContextSelected.Args>
    {
        public class Args
        {
            public string NewContextSelection
            {
                get;
                set;
            }

            public string OldContextSelection
            {
                get;
                set;
            }
        }

        public ContextSelected():base(){}
    }
}
