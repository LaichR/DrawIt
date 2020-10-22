using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Commands;
using UI.Utilities;
using UI.Utilities.Interfaces;
using UI.Utilities.Behaviors;
using Sketch;
using Sketch.Types;
using Sketch.Models;
using DrawIt.Shapes;
using DrawIt.Properties;
using Sketch.Interface;
using Sketch.Models.BasicItems;

namespace DrawIt.Uml
{
    class UmlShapeFactory : BindableBase, ISketchItemFactory
    {
        readonly SketchItemFactory _factory = new SketchItemFactory();

        public UmlShapeFactory()
        {
            _factory.RegisterSketchItem(typeof(UmlClassModel), "Class", "Add new class", Properties.Resources.UmlClassShape);
            _factory.RegisterSketchItem(typeof(UmlPackageModel), "Package", "Add new package", Properties.Resources.UmlPackageShape);
            _factory.RegisterSketchItem(typeof(UmlStateModel), "State", "Add a new state", Properties.Resources.UmlStateShape);
            _factory.RegisterSketchItem(typeof(UmlChoiceModel), "Decision Point", "Add a new decision point", Properties.Resources.UmlChoiceShape);
            _factory.RegisterSketchItem(typeof(UmlInitialStateModel), "Init State", "Add a init state", Properties.Resources.UmlInitialStateShape);
            _factory.RegisterSketchItem(typeof(UmlFinalStateModel),"Final State", "Add a final state", Properties.Resources.UmlFinalStateShape);
            _factory.RegisterSketchItem(typeof(UmlTransitionModel), "Transition", "Add a transition", Properties.Resources.UmlAssociationShape);
            _factory.RegisterSketchItem(typeof(UmlAssociationModel), "Association", "Add a association", Properties.Resources.UmlAssociationShape);
            _factory.RegisterSketchItem(typeof(UmlGeneralizationModel), "Generalisation", "Define base", Properties.Resources.UmlGeneralisationShape);
            _factory.RegisterSketchItem(typeof(UmlMessageModel), "Message", "Add a Message", Properties.Resources.UmlAssociationShape);
            _factory.RegisterSketchItem(typeof(UmlCompositionModel), "Composition", "Add a composition", Properties.Resources.UmlCompositionShape);
            _factory.RegisterSketchItem(typeof(UmlDependencyModel), "Dependency", "Add a dependency", Properties.Resources.UmlDependencyShape);
            _factory.RegisterSketchItem(typeof(UmlNoteModel), "Note", "Add a note", Properties.Resources.UmlNoteShape);
            _factory.RegisterSketchItem(typeof(UmlActionConnector), "Connector", "Add an ActionConnector", Properties.Resources.UmlActionConnectorShape);
            _factory.RegisterSketchItem(typeof(UmlActivityModel), "Action", "Add an Action", Properties.Resources.UmlActionShape);
            _factory.RegisterSketchItem(typeof(UmlActivityDiagramEdge), "Activity Diagram Edge", "Add edge to next action or activity", Properties.Resources.UmlAssociationShape);
            _factory.RegisterSketchItem(typeof(FreeTextModel), "Free text element", "Add a text element to the diagram", Sketch.Properties.Resources.free_text);
            _factory.RegisterSketchItem(typeof(UmlLifeLineModel), "Life line", "Add an object life line", Properties.Resources.LifeLine);
            _factory.SetInitialSelection(  typeof(UmlInitialStateModel) );
        }


        public void RegisterBoundedItemSelectedNotification(EventHandler handler)
        {
            _factory.RegisterBoundedItemSelectedNotification(handler);
        }
        public void UnregisterBoundedItemSelectedNotification(EventHandler handler)
        {
            _factory.UnregisterBoundedItemSelectedNotification(handler);
        }
        public void RegisterConnectorItemSelectedNotification(EventHandler handler)
        {
            _factory.RegisterConnectorItemSelectedNotification(handler);
        }

        public void UnregisterConnectorItemSelectedNotification(EventHandler handler)
        {
            _factory.UnregisterConnectorItemSelectedNotification(handler);
        }

        public Type SelectedForCreation =>  _factory.SelectedForCreation;

        public IList<ICommandDescriptor> Palette => _factory.Palette;

        public IBoundedItemModel CreateConnectableSketchItem(Type cls, Point p)
        {
            return _factory.CreateConnectableSketchItem(cls, p);
        }

        public IConnectorItemModel CreateConnector(Type cls, ConnectionType type, 
            IBoundedItemModel from, IBoundedItemModel to,
            Point startPointHint, Point endPointHint,
            ISketchItemContainer container)
        {
            return _factory.CreateConnector(cls, type, from, to, startPointHint, endPointHint, container);
        }

        public IList<ICommandDescriptor> GetAllowableConnctors(Type t)
        {
            return _factory.GetAllowableConnctors(t);
        }

        public IList<IBoundedItemFactory> GetConnectableFactories(Type t)
        {
            return _factory.GetConnectableFactories(t);
        }
    }
}
