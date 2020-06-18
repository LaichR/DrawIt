using Sketch.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace Sketch.Controls
{
    internal interface IGadgetUI
    {
        bool IsSelected
        {
            get;
            set;
        }

        bool IsMarked
        {
            get;
            set;
        }
        
        event EventHandler<bool> SelectionChanged;
        event EventHandler<bool> IsMarkedChanged;

        Rect LabelArea
        {
            get;
        }

        Shape Shape
        {
            get;
        }

        Models.ModelBase Model
        {
            get;
        }

        ISketchItemModel RefModel
        {
            get;
        }

        void Disable();

        void Enable();

        void TriggerSnapshot();

        void DropSnapshot();

    }
}
