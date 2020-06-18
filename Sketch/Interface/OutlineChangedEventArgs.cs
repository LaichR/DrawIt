using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sketch.Interface
{
    public class OutlineChangedEventArgs:EventArgs
    {
        public OutlineChangedEventArgs(Rect oldBounds, Rect newBounds ) 
        {
            OldBounds = oldBounds;
            NewBounds = newBounds;
        }

        public Rect OldBounds
        {
            get;
            private set;
        }

        public Rect NewBounds
        {
            get;
            private set;
        }

    }

}
