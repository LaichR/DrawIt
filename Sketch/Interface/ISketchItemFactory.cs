using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sketch.Models;
using Sketch.Types;
using System.Windows;
using UI.Utilities.Interfaces;


namespace Sketch.Interface
{

    public interface ISketchItemFactory
    {
        Type SelectedForCreation
        {
            get;
        }

        void RegisterBoundedItemSelectedNotification(EventHandler handler);
        void UnregisterBoundedItemSelectedNotification(EventHandler handler);
        void RegisterConnectorItemSelectedNotification(EventHandler handler);
        void UnregisterConnectorItemSelectedNotification(EventHandler handler);


        IBoundedItemModel CreateConnectableSketchItem(Type cls, Point p);
        
        IConnectorItemModel CreateConnector(Type cls, ConnectionType type, IBoundedItemModel from, IBoundedItemModel to, 
            Point startPointHint, Point endPointHint,
            ISketchItemContainer container );

        IList<IBoundedItemFactory> GetConnectableFactories(Type t);

        IList<ICommandDescriptor> GetAllowableConnctors(Type t);

        IList<ICommandDescriptor> Palette
        {
            get;
        }
    }
}
