using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Events;
using System.Threading.Tasks;

namespace Bluebottle.Base.Events
{


    public class ScriptSelected : PubSubEvent<ScriptSelected.Args>
    {
        public enum Purpose
        {
            Execute,
            Open
        }

        public class Args
        {
            public Args(string scriptPath, Purpose purpose )
            {
                ScriptPath = scriptPath;
                Purpose = purpose;
            }

            public string ScriptPath
            {
                get;
                private set;
            }

            public Purpose Purpose
            {
                get;
                private set;
            }
        }

        public ScriptSelected():base(){}
    }
}
