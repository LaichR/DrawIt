using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bluebottle.Base.Interfaces
{
    public interface ITextAccessor
    {
        string GetText();
        void SetText(string text);
    }
}
