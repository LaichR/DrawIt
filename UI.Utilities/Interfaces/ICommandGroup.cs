using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bluebottle.Base.Interfaces
{
    public interface ICommandGroup
    {
        string Name
        {
            get;
        }

        IList<ICommandDescriptor> Commands
        {
            get;
        }
    }
}
