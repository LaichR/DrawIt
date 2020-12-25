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
    /// <summary>
    /// This delegate reflects the signature that is used to create any new connector item
    /// </summary>
    /// <param name="connectionType"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="container"></param>
    /// <returns></returns>
    delegate IConnectorItemModel CreateConnectorDelegate(ConnectionType
        connectionType, IBoundedSketchItemModel from, IBoundedSketchItemModel to,
        Point start, Point end,
        ISketchItemContainer container);

    /// <summary>
    /// A connector item draws a visible connection between two connectable items.
    /// 
    /// </summary>
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
