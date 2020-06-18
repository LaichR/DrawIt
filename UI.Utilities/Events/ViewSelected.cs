using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;


namespace Bluebottle.Base.Events
{
    public class ViewSelected: PubSubEvent<ViewSelected.Args>
    {
        public class Args
        {
            public Args(string contextId, string viewName)
            {
                ContextId = contextId;
                ViewName = viewName;

            }

            public string ContextId
            {
                get;
                private set;
            }

            public string ViewName
            {
                get;
                private set;
            }
        }

        public ViewSelected() : base() { }
    }
}
