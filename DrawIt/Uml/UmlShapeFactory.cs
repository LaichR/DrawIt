﻿using System;
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
   
        

        class UmlShapeFactory : SketchItemFactory
        {


        public UmlShapeFactory() : base("misc shapes")
        { }

        public override void InitializeFactory()
        {
            var category = "Class Diagram";
            RegisterSketchItemInCategory(category,typeof(UmlClassModel), "Class", "Add new class", Properties.Resources.UmlClassShape);
            RegisterSketchItemInCategory(category,typeof(UmlPackageModel), "Package", "Add new package", Properties.Resources.UmlPackageShape);
            RegisterSketchItemInCategory(category,typeof(UmlCompositionModel), "Composition", "Add a composition", Properties.Resources.UmlCompositionShape);
            RegisterSketchItemInCategory(category,typeof(UmlDependencyModel), "Dependency", "Add a dependency", Properties.Resources.UmlDependencyShape);
            RegisterSketchItemInCategory(category,typeof(UmlNoteModel), "Note", "Add a note", Properties.Resources.UmlNoteShape);
            RegisterSketchItemInCategory(category,typeof(UmlAssociationModel), "Association", "Add a association", Properties.Resources.UmlAssociationShape);
            RegisterSketchItemInCategory(category,typeof(UmlGeneralizationModel), "Generalisation", "Define base", Properties.Resources.UmlGeneralisationShape);

            category = "Activity Diagram";

            RegisterSketchItemInCategory(category, typeof(UmlNoteModel), "Note", "Add a note", Properties.Resources.UmlNoteShape);
            RegisterSketchItemInCategory(category, typeof(UmlChoiceModel), "Decision Point", "Add a new decision point", Properties.Resources.UmlChoiceShape);
            RegisterSketchItemInCategory(category, typeof(UmlInitialStateModel), "Init State", "Add a init state", Properties.Resources.UmlInitialStateShape);
            RegisterSketchItemInCategory(category, typeof(UmlFinalStateModel),"Final State", "Add a final state", Properties.Resources.UmlFinalStateShape);
            RegisterSketchItemInCategory(category, typeof(UmlActionConnector), "Connector", "Add an ActionConnector", Properties.Resources.UmlActionConnectorShape);
            RegisterSketchItemInCategory(category, typeof(UmlActivityModel), "Action", "Add an Action", Properties.Resources.UmlActionShape);
            RegisterSketchItemInCategory(category, typeof(UmlActivityDiagramEdge), "Activity Diagram Edge", "Add edge to next action or activity", Properties.Resources.UmlAssociationShape);

            category = "Sequence Diagram";
            RegisterSketchItemInCategory(category,typeof(UmlMessageModel), "Message", "Add a Message", Properties.Resources.UmlAssociationShape);
            RegisterSketchItemInCategory(category, typeof(UmlLifeLineModel), "Life line", "Add an object life line", Properties.Resources.LifeLine);
            

            category = "State Diagram";
            RegisterSketchItemInCategory(category, typeof(UmlNoteModel), "Note", "Add a note", Properties.Resources.UmlNoteShape);
            RegisterSketchItemInCategory(category, typeof(UmlStateModel), "State", "Add a new state", Properties.Resources.UmlStateShape);
            RegisterSketchItemInCategory(category, typeof(UmlChoiceModel), "Decision Point", "Add a new decision point", Properties.Resources.UmlChoiceShape);
            RegisterSketchItemInCategory(category, typeof(UmlInitialStateModel), "Init State", "Add a init state", Properties.Resources.UmlInitialStateShape);
            RegisterSketchItemInCategory(category, typeof(UmlFinalStateModel), "Final State", "Add a final state", Properties.Resources.UmlFinalStateShape);
            RegisterSketchItemInCategory(category, typeof(UmlTransitionModel), "Transition", "Add a transition", Properties.Resources.UmlAssociationShape);


            category = "Miscellaneous";
            RegisterSketchItemInCategory(category, typeof(FreeTextModel), "Free text element", "Add a text element to the diagram", Sketch.Properties.Resources.free_text);
            SetInitialSelection(  typeof(UmlInitialStateModel) );
        }


    }
}
