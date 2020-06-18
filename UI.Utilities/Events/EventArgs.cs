using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bluebottle.Base.Events
{
    public class EventArgs<T>: EventArgs
    {
        T _value;

        public EventArgs( T value)
        {
            _value = value;
        }

        public T Value
        {
            get
            {
                return _value;
            }
        }
    }
}
