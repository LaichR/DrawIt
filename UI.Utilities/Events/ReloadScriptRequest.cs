using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bluebottle.Base.Events
{
    public class ReloadScriptRequest : PubSubEvent<FileSystemEventArgs>
    {
        public ReloadScriptRequest() : base() { }
    }
}
