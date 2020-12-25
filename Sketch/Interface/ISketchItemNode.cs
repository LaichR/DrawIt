using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sketch.Interface
{
    /// <summary>
    /// Base interface for all elements in the object tree.
    /// </summary>
    public interface ISketchItemNode
    {
        /// <summary>
        /// Reference to the Parent of this node
        /// </summary>
        ISketchItemNode ParentNode
        {
            get;
        }

        /// <summary>
        /// Label of a node
        /// </summary>
        string Label
        {
            get;
            set;
        }

        /// <summary>
        /// The lable can be set to readonly
        /// </summary>
        bool CanEditLabel
        {
            get;
        }

    }
}
