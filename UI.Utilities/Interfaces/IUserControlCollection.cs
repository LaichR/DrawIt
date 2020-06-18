using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Bluebottle.Base.Interfaces
{
    /// <summary>
    /// this interface is used to manage user controls of the same type accross different views
    /// the original purpose of this interface is to group all editors of the same type to allow to search them and to share the same
    /// actions accross all of them.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUserControlCollection<T> where T: UserControl
    {
        T FocusedControl
        {
            get;
            set;
        }

        Dictionary<string, T> CollectionControls
        {
            get;
        }

        void RegisterEditor(string key, T editor);

    }
}
