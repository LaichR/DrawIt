using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Bluebottle.Base.Events
{
    public class PublishTools: PubSubEvent<PublishTools.Args>
    {
        public class Args
        {
            public Args( string contextId, ToolBar tools)
            {
                ContextId = contextId;
                ToolBar = tools;
            }

            public string ContextId
            {
                get;
                private set;
            }

            public ToolBar ToolBar
            {
                get;
                private set;
            }
        }

        public PublishTools():base(){}
    }

    public class RevokeTools: PubSubEvent<PublishTools.Args>
    {
        public RevokeTools() : base() { }
    }
}
