using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Bluebottle.Base.Interfaces;

namespace Bluebottle.Base.Controls
{
    public class CommandGroupContainerMenuItem: MenuItem
    {

        List<Tuple<ICommandGroup, List<MenuItem>>> _menuItems = new List<Tuple<ICommandGroup, List<MenuItem>>>();

        public void AddMeunItemsOfGroup(ICommandGroup group)
        {
            if( GetRegistered(group) == null)
            {
                List<MenuItem> menuItems = new List<MenuItem>();
                foreach( var cd in group.Commands)
                {
                    var mi = new MenuItem()
                    {
                        Command = cd.Command,
                        Header = cd.Name,
                        ToolTip = cd.ToolTip,
                    };
                    menuItems.Add(mi);
                    this.Items.Add(mi);
                }
                this._menuItems.Add(new Tuple<ICommandGroup, List<MenuItem>>(group, menuItems));
            }
        }

        public void RemoveMenuItemsOfGroup(ICommandGroup group)
        {
            var groupMenuItemAssociation = GetRegistered(group);
            if (groupMenuItemAssociation != null)
            {
                _menuItems.Remove(groupMenuItemAssociation);
                foreach( var mi in groupMenuItemAssociation.Item2)
                {
                    this.Items.Remove(mi);
                }
            }
        }

        Tuple<ICommandGroup, List<MenuItem>> GetRegistered( ICommandGroup group )
        {
            foreach (var tuple in _menuItems)
            {
                if (tuple.Item1 == group) return tuple;
            }
            return null;
        }

        
    }
}
