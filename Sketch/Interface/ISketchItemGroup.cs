using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sketch.Helper.Binding;

namespace Sketch.Interface
{
    /// <summary>
    /// Allows to create a palette of actions that create items that belong to the same group or categorie.
    /// Like this it becomes possible to create groups within the SketchItemFactory
    /// </summary>
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
