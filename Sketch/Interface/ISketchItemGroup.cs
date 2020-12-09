using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Utilities.Interfaces;

namespace Sketch.Interface
{
    public interface ISketchItemGroup
    {
        string Name
        {
            get;
        }

        void AddType(Type type);

        IList<ICommandDescriptor> Palette
        {
            get;
        }
    }
}
