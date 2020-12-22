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
        
        void SaveFile(string fileName);

        void OpenFile(string fileName);

        //double Scaling
        //{
        //    get;
        //    set;
        //}

        void AlignLeft();

        void AlignTop();

        void AlignCenter();

        void SetToSameWidth();

        void SetEqualVerticalSpacing();

        void ZoomIn();

        void ZoomOut();

    }
}
