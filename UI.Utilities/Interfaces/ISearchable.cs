using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScintillaNET;

namespace Bluebottle.Base.Interfaces
{
    public enum FindAndReplaceOperation { Find, Replace }

    public interface ISearchable
    {
        void SearchText(string text, SearchFlags flags);

        event EventHandler<FindAndReplaceOperation> FindAndReplaceRequest;
    }
}
