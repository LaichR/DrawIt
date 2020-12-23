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
        connectionType, IBoundedSketchItemModel from, IBoundedSketchItemModel to,
        Point start, Point end,
        ISketchItemContainer container);

    public interface IConnectorItemModel: ISketchItemModel
    {
        IBoundedSketchItemModel From
        {
            get;
        }

        IBoundedSketchItemModel To
        {
            get;
        }

        bool AllowWaypoints
        {
            get;
        }
    }
}
