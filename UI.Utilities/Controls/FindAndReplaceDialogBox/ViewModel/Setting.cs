using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bluebottle.Base.Controls.FindAndReplaceDialogBox.ViewModel
{
    public class Setting: BindableBase
    {

        public Setting(string name)
        {
            Name = name;
        }

        public Setting(string name, bool isChecked)
        {
            Name = name;
            IsChecked = isChecked;
        }

        public string Name { get; set; }

        public bool IsChecked
        { 
            get;
            set;
        }

    }
}
