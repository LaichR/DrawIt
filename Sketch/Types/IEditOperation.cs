using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Windows.Input;

namespace Sketch.Controls
{
    public interface IEditOperation
    {
        //void Start();
        
        void StopOperation(bool commit );

       
    }
}
