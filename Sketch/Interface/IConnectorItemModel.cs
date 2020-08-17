using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace Sketch.Interface
{
    delegate IConnectorItemModel CreateConnectorDelegate(ConnectionType
        connectionType, IBoundedItemModel from, IBoundedItemModel to,
        ISketchItemContainer container);

    public interface IConnectorItemModel: ISketchItemModel
    {
        IBoundedItemModel From
        {
            get;
        }

        IBoundedItemModel To
        {
            get;
        }


    }
}
