using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Windows.Input;

namespace Sketch.Controls
{
    internal interface IEditOperation
    {
        //void Start();
        
        void StopOperation(bool ok);

       
    }
}
