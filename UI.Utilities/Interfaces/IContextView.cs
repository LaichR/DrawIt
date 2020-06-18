using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bluebottle.Base.Interfaces
{
    public interface IContextView
    {
        string Name
        {
            get;
        }

        void Activate(string contextId);

        void Deactivate();
    }
}
