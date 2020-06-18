using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Bluebottle.Base.Interfaces;

namespace Bluebottle.Base.Controls
{
    public class CommandGroupContainerToolBar: ToolBar
    {
        ICommandGroup _cmdGroup;

        public CommandGroupContainerToolBar(ICommandGroup group)
        {
            HIMS.Services.Core.Contract.Requires<ArgumentNullException>(group != null);
            _cmdGroup = group;
            foreach (var cm in _cmdGroup.Commands)
            {
                Control control;

                if (!cm.IsSelector)
                {
                    control = new Bluebottle.Base.Controls.ToolbarButton(cm, cm.Bitmap,
                    new Dictionary<string, System.Windows.DependencyProperty>
                    {
                        { "Command", Bluebottle.Base.Controls.ToolbarButton.CommandProperty },
                        { "Background", Bluebottle.Base.Controls.ToolbarButton.BackgroundProperty },
                        { "ToolTip", Bluebottle.Base.Controls.ToolbarButton.ToolTipProperty }
                    });
                }
                else
                {
                    control = new Bluebottle.Base.Controls.ToolbarComboBox { ToolTip = cm.ToolTip };
                }

                cm.Initialize?.Invoke(control);
                Items.Add(control);
            }
            VerticalAlignment = System.Windows.VerticalAlignment.Top;
        }

        public ICommandGroup CmdGroup
        {
            get
            {
                return _cmdGroup;
            }
        }


    }
}
