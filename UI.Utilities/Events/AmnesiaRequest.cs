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
    public class AmnesiaRequest: PubSubEvent<AmnesiaRequest.Args>
    {
        public enum Module
        {
            ShellInterpreter = 0x01,
            Contexts = 0x02
        }

        public class Args
        {
            public Args(object sender, Module module)
            {
                Module = module;
                Sender = sender;
            }

            public Module Module
            {
                get;
                private set;
            }

            public object Sender
            {
                get;
                private set;
            }
        }

        public AmnesiaRequest() : base() { }
    }
}
