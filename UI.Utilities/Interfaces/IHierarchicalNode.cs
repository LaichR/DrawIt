using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Utilities.Interfaces
{
    public interface IHierarchicalNode
    {
        IHierarchicalNode Parent
        {
            get;
        }

        string Name
        {
            get;
        }

        bool AllowEdit
        {
            get;
            set;
        }
    }
}
