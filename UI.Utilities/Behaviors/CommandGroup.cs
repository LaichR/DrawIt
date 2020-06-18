using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bluebottle.Base.Interfaces;

namespace Bluebottle.Base.Behaviors
{
    public class CommandGroup: ICommandGroup
    {

        List<ICommandDescriptor> _commands = new List<ICommandDescriptor>();

        public CommandGroup( string name, params ICommandDescriptor[] commands)
        {
            _commands.AddRange(commands);
            Name = name;
        }

        public string Name
        {
            get;
            private set;
        }

        public IList<ICommandDescriptor> Commands
        {
            get { return _commands; }
        }
    }
}
