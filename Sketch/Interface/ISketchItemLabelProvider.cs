using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sketch.Interface
{
    public interface ISketchItemNode
    {
        ISketchItemNode ParentNode
        {
            get;
        }

        string Label
        {
            get;
            set;
        }

        bool CanEditLabel
        {
            get;
        }

    }
}
