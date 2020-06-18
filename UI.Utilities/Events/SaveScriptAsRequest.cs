using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;


namespace Bluebottle.Base.Events
{
    public class SaveScriptAsRequest: PubSubEvent<SaveScriptAsRequest.Args>
    {
        public class Args
        {
            public Args(object editor)
            {
                this.Editor = editor;
            }

            public object Editor
            {
                get;
                private set;
            }
        }

        public SaveScriptAsRequest() : base() { }
    }
}
