using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sketch.Interface
{
    public interface ISketchPadControl
    {
        void ExportDiagram(string fileName);
        
        void SaveFile(string fileName, bool silent);

        void OpenFile(string fileName);

        void AlignLeft();

        void AlignTop();

        void AlignCenter();

        void ZoomIn();

        void ZoomOut();

    }
}
