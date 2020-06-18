using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;


namespace Bluebottle.Base.Events
{
    public class ApplicationTrace: PubSubEvent<ApplicationTrace.Args>
    {
        public enum Severity { None, Error, Log, Success, Info, Warning, _N }

        public class Args
        {
            StringBuilder _msg;
            public Args(Severity severity, string format, params object[] p)
            {
                this.Severity = severity;
                _msg = new StringBuilder();
                _msg.AppendFormat(format, p);
                if (!format.EndsWith("\n"))
                    _msg.Append("\n");
            }

            public Severity Severity
            {
                get;
                private set;
            }

            public string Message
            {
                get { return _msg.ToString(); }
            }
        }

        public ApplicationTrace() : base() { }
    }
}
