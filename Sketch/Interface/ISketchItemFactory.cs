using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sketch.Models;
using Sketch.Helper;
using System.Windows;
using UI.Utilities.Interfaces;


namespace Sketch.Interface
{
    /// <summary>
    /// Abstract factory for Connectable Items and Connectors. 
    /// </summary>
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


        IBoundedSketchItemModel CreateConnectableSketchItem(Type cls, Point p, ISketchItemContainer container);
        
        IConnectorItemModel CreateConnector(Type cls, IBoundedSketchItemModel from, IBoundedSketchItemModel to, 
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
