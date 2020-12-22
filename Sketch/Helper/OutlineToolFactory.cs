using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Utilities.Interfaces;

namespace Sketch.Helper
{
    [Serializable]
    public abstract class OutlineToolFactory
    {
        public abstract IList<ICommandDescriptor> GetTools();
    }
}
