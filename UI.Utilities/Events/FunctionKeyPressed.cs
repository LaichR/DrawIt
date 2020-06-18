using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Events;

namespace Bluebottle.Base.Events
{
    public class FunctionKeyPressed : PubSubEvent<FunctionKeyPressed.Args>
    {
        public class Args
        {
            public Args(object editor, Key functionKey)
            {
                this.Editor = editor;
                FunctinoKey = functionKey;
            }

            public object Editor
            {
                get;
                private set;
            }

            public Key FunctinoKey
            {
                get;
                private set;
            }
        }
    }
}
