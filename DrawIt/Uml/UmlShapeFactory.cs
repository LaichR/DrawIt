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

namespace DrawIt.Uml
{
    class UmlShapeFactory : BindableBase, ISketchItemFactory
    {


        //    [Serializable]
        //    class ClassToolFactory: OutlineToolFactory
        //    {

        //        public ClassToolFactory() { }

        //        static List<ICommandDescriptor> _classTools;
        //        internal ClassToolFactory(UmlShapeFactory shapeFactory)
        //        {
        //            _classTools = new List<ICommandDescriptor>
        //                {
        //                    new UI.Utilities.Behaviors.CommandDescriptor{ 
        //                            Bitmap = Properties.Resources.UmlAssociationShape,
        //                            Name = "Association", Command = shapeFactory._selectAssociation},

        //                    new UI.Utilities.Behaviors.CommandDescriptor{ 
        //                            Bitmap = Properties.Resources.UmlCompositionShape,
        //                            Name = "Composition", Command = shapeFactory._selectComposition},

        //                    new UI.Utilities.Behaviors.CommandDescriptor{ 
        //                            Bitmap = Properties.Resources.UmlGeneralisationShape,
        //                            Name = "Generalisation", Command = shapeFactory._selectGeneralisation},

        //                    new UI.Utilities.Behaviors.CommandDescriptor{ 
        //                            Bitmap = Properties.Resources.UmlDependencyShape,
        //                            Name = "Dependency", Command = shapeFactory._selectDependency},

        //                };
        //        }

        //        public override IList<ICommandDescriptor> GetTools()
        //        {
        //            return _classTools;
        //        }
        //    }

        //    [Serializable]
        //    class PackageToolFactory: OutlineToolFactory
        //    {
        //        static List<ICommandDescriptor> _packageTools;
        //        internal PackageToolFactory(UmlShapeFactory shapeFactory)
        //        {
        //            _packageTools = new List<ICommandDescriptor>
        //                {
        //                    new UI.Utilities.Behaviors.CommandDescriptor{ 
        //                            Bitmap = Properties.Resources.UmlAssociationShape,
        //                            Name = "Association", Command = shapeFactory._selectAssociation},

        //                    //new UI.Utilities.Behaviors.CommandDescriptor{ 
        //                    //        Bitmap = Properties.Resources.UmlCompositionShape,
        //                    //        Name = "Composition", Command = shapeFactory._selectComposition},



        //                    new UI.Utilities.Behaviors.CommandDescriptor{ 
        //                            Bitmap = Properties.Resources.UmlDependencyShape,
        //                            Name = "Dependency", Command = shapeFactory._selectDependency},

        //                };
        //        }


        //        public override IList<ICommandDescriptor> GetTools()
        //        {
        //            return _packageTools;
        //        }
        //    }

        //    [Serializable]
        //    class PictureToolFactory : OutlineToolFactory
        //    {
        //        static List<ICommandDescriptor> _pictureTools;
        //        internal PictureToolFactory(UmlShapeFactory shapeFactory)
        //        {
        //            _pictureTools = new List<ICommandDescriptor>
        //                {
        //                    new UI.Utilities.Behaviors.CommandDescriptor{ 
        //                            Bitmap = Properties.Resources.UmlAssociationShape,
        //                            Name = "Association", Command = shapeFactory._selectAssociation},

        //                };
        //        }


        //        public override IList<ICommandDescriptor> GetTools()
        //        {
        //            return _pictureTools;
        //        }
        //    }

        //    [Serializable]
        //    class StateToolFactory : OutlineToolFactory
        //    {
        //        static List<ICommandDescriptor> _stateTools;
        //        internal StateToolFactory(UmlShapeFactory shapeFactory)
        //        {
        //            _stateTools = new List<ICommandDescriptor>
        //                {
        //                    new UI.Utilities.Behaviors.CommandDescriptor{ 
        //                            Bitmap = Properties.Resources.UmlAssociationShape,
        //                            Name = "Transition", Command = shapeFactory._selectTransition},

        //                };
        //        }


        //        public override IList<ICommandDescriptor> GetTools()
        //        {
        //            return _stateTools;
        //        }
        //    }

        //    [Serializable]
        //    class InitStateToolFactory : OutlineToolFactory
        //    {
        //        static List<ICommandDescriptor> _stateTools;
        //        internal InitStateToolFactory(UmlShapeFactory shapeFactory)
        //        {
        //            _stateTools = new List<ICommandDescriptor>
        //                {
        //                    new UI.Utilities.Behaviors.CommandDescriptor{
        //                            Bitmap = Properties.Resources.UmlAssociationShape,
        //                            Name = "Transition", Command = shapeFactory._selectTransition},

        //                };
        //        }

        //        public override IList<ICommandDescriptor> GetTools()
        //        {
        //            return _stateTools;
        //        }
        //    }

        //    [Serializable]
        //    class ChoiceToolFactory : OutlineToolFactory
        //    {
        //        static List<ICommandDescriptor> _choinceTools;
        //        internal ChoiceToolFactory(UmlShapeFactory shapeFactory)
        //        {
        //            _choinceTools = new List<ICommandDescriptor>
        //                {
        //                    new UI.Utilities.Behaviors.CommandDescriptor{
        //                            Bitmap = Properties.Resources.UmlAssociationShape,
        //                            Name = "Transition", Command = shapeFactory._selectTransition},
        //                    new UI.Utilities.Behaviors.CommandDescriptor
        //                    {
        //                        Bitmap = Properties.Resources.UmlDependencyShape,
        //                        Name = "Dependency", Command = shapeFactory._selectDependency
        //                    }
        //                };
        //        }

        //        public override IList<ICommandDescriptor> GetTools()
        //        {
        //            return _choinceTools;
        //        }
        //    }


        //    ICommand _selectClass;
        //    ICommand _selectPackage;
        //    ICommand _selectChoice;
        //    ICommand _selectLifeLine;
        //    ICommand _selectNote;
        //    ICommand _selectFinalState;
        //    ICommand _selectInitialState;
        //    ICommand _selectState;
        //    ICommand _selectPicture;
        //    ICommand _selectDefaultConnector;
        //    ICommand _selectAssociation;
        //    ICommand _selectTransition;
        //    ICommand _selectDependency;
        //    ICommand _selectComposition;
        //    ICommand _selectGeneralisation;


        //    List<ICommandDescriptor> _tools;

        //    ConnectableBaseFactory _classFactory;
        //    ConnectableBaseFactory _packageFactory;
        //    ConnectableBaseFactory _stateFactory;
        //    ConnectableBaseFactory _noteFactory;
        //    ConnectableBaseFactory _choiceFactory;
        //    ConnectableBaseFactory _initialStateFactory;
        //    ConnectableBaseFactory _finalStateFactory;
        //    ConnectableBaseFactory _pictureFactory;


        //    List<IBoundedItemFactory> _associationOrComposition;
        //    List<IBoundedItemFactory> _inheritance;
        //    List<IBoundedItemFactory> _transition;
        //    List<IBoundedItemFactory> _dependency;
        //    List<IBoundedItemFactory> _stateActionOrChoice;



        //    UmlElementType _selectItemState;
        //    UmlElementType _selectConnectorState;
        //    ClassToolFactory _classToolsFactory;
        //    PackageToolFactory _packageToolFactory;
        //    StateToolFactory _stateToolFactory;
        //    PictureToolFactory _pictureToolFactory;
        //    InitStateToolFactory  _initialStateToolFactory;
        //    ChoiceToolFactory _choiceToolFactory;


        //    public UmlShapeFactory()
        //    {
        //        CreateConnectorEndFactories();

        //        _selectChoice = new DelegateCommand(() =>
        //        {
        //            NotifyOnSelectElement(UmlElementType.typeChoice);
        //        });

        //        //_selectLifeLine = new DelegateCommand(() =>
        //        //    NotifyOnSelectElement(UmlElementType.typeLineLine)
        //        //);

        //        _selectClass = new DelegateCommand(() =>
        //        {
        //            NotifyOnSelectElement(UmlElementType.typeClass);
        //        });

        //        _selectPackage = new DelegateCommand(() =>
        //        {
        //            NotifyOnSelectElement(UmlElementType.typePackage);
        //        });

        //        _selectNote = new DelegateCommand(() =>
        //        {
        //            NotifyOnSelectElement(UmlElementType.typeNote);
        //        });

        //        _selectInitialState = new DelegateCommand(() =>
        //        {
        //            NotifyOnSelectElement(UmlElementType.typeInitialState);
        //        });

        //        _selectFinalState = new DelegateCommand(() =>
        //        {
        //            NotifyOnSelectElement(UmlElementType.typeFinalState);
        //        });

        //        _selectState = new DelegateCommand(() =>
        //        {
        //            NotifyOnSelectElement(UmlElementType.typeState);
        //        });

        //        _selectPicture = new DelegateCommand(() =>
        //        {
        //            NotifyOnSelectElement(UmlElementType.typePicture);
        //        });

        //        _selectDefaultConnector = new DelegateCommand(() =>
        //        {
        //            NotifyOnSelectConnector(UmlElementType.typeDefaultConnector);
        //        });

        //        _selectAssociation = new DelegateCommand(() =>
        //        {
        //            NotifyOnSelectConnector(UmlElementType.typeAssociation);
        //        });

        //        _selectTransition = new DelegateCommand(() =>
        //        {
        //            NotifyOnSelectConnector(UmlElementType.typeTransition);
        //        });

        //        _selectComposition = new DelegateCommand(() =>
        //        {
        //            NotifyOnSelectConnector(UmlElementType.typeComposition);
        //        });

        //        _selectGeneralisation = new DelegateCommand(() =>
        //        {
        //            NotifyOnSelectConnector(UmlElementType.typeGeneralisation);
        //        });

        //        _selectDependency = new DelegateCommand(() =>
        //        {
        //            NotifyOnSelectConnector(UmlElementType.typeDependency);
        //        });

        //        _tools = new List<ICommandDescriptor>
        //        {
        //            new CommandDescriptor{Name="Package", Command=SelectPackage, Bitmap=Resources.UmlPackageShape},
        //            new CommandDescriptor{Name="Class", Command=SelectClass, Bitmap=Resources.UmlClassShape},
        //            new CommandDescriptor{Name="State", Command=SelectState, Bitmap=Resources.UmlStateShape},
        //            new CommandDescriptor{Name="Choice", Command=SelectChoice, Bitmap=Resources.UmlChoiceShape},
        //            //new CommandDescriptor{Name="Life Line", Command=SelectLifeLine, Bitmap=Resources.UmlStateShape},
        //            new CommandDescriptor{Name="Note", Command=SelectNote, Bitmap=Resources.UmlNoteShape},
        //            new CommandDescriptor{Name="InitalState", Command = SelectInitialState, Bitmap=Resources.UmlInitialStateShape },
        //            new CommandDescriptor{Name="FinalState", Command = SelectFinalState, Bitmap=Resources.UmlFinalStateShape },
        //            new CommandDescriptor{Name="Picture", Command=_selectPicture, Bitmap=Resources.image}
        //        };

        //        _classToolsFactory = new ClassToolFactory(this);
        //        _packageToolFactory = new PackageToolFactory(this);
        //        _stateToolFactory = new StateToolFactory(this);
        //        _pictureToolFactory = new PictureToolFactory(this);
        //        _initialStateToolFactory = new InitStateToolFactory(this);
        //        _choiceToolFactory = new ChoiceToolFactory(this);

        //        //_packageTools = new List<ICommandDescriptor> 
        //        //{
        //        //     new UI.Utilities.Behaviors.CommandDescriptor{ 
        //        //            Bitmap = Sketch.Properties.Resources.DefaultConnector,
        //        //            Name = "DefaultConnector", Command = _selectDefaultConnector} 
        //        //};

        //    }

        //    public IBoundedItemModel CreateConnectableSketchItem(System.Windows.Size parentSize, System.Windows.Point p)
        //    {
        //        List<ICommandDescriptor> connectorTools = new List<ICommandDescriptor>
        //        {
        //            new UI.Utilities.Behaviors.CommandDescriptor
        //            {
        //                Bitmap = Sketch.Properties.Resources.DefaultConnector,
        //                Name = "DefaultConnector", Command = _selectDefaultConnector},
        //            new UI.Utilities.Behaviors.CommandDescriptor
        //            {
        //                Bitmap = Properties.Resources.UmlAssociationShape,
        //                Name = "Association ", Command = _selectAssociation}
        //        };

        //        switch( this._selectItemState)
        //        {
        //            case UmlElementType.typePackage:
        //                return new UmlPackageModel(p, _packageToolFactory);
        //            case UmlElementType.typeClass:
        //                return new UmlClassModel(p, _classToolsFactory);
        //            case UmlElementType.typeState:
        //                return new UmlStateModel(p, _stateToolFactory);
        //            case UmlElementType.typeNote:
        //                return new UmlNoteModel(p, null);
        //            case UmlElementType.typeChoice:
        //                return new UmlChoiceModel(p, _choiceToolFactory);
        //            case UmlElementType.typeFinalState:
        //                return new UmlFinalStateModel(p, null);
        //            case UmlElementType.typeInitialState:
        //                return new UmlInitialStateModel(p, _initialStateToolFactory);
        //            //case UmlElementType.typeLineLine:
        //            //    return new UmlLifeLineModel(parentSize, p, _classToolsFactory);
        //            case UmlElementType.typePicture:
        //                return doCreatePicture( p );
        //            default:
        //                return new Sketch.Models.ContainerModel(p, new Size(150, 75), _stateToolFactory);
        //        }

        //    }

        //    public IConnectorItemModel CreateConnector(Sketch.Interface.ConnectionType type, 
        //        IBoundedItemModel from, IBoundedItemModel to)
        //    {
        //        switch( _selectConnectorState)
        //        {
        //            case UmlElementType.typeAssociation:
        //                return new UmlAssociationModel(type, from, to);
        //            case UmlElementType.typeComposition:
        //                return new UmlCompositionModel(type, from, to);
        //            case UmlElementType.typeGeneralisation:
        //                return new UmlGeneralizationModel(type, from, to);
        //            case UmlElementType.typeDependency:
        //                return new UmlDependencyModel(Sketch.Interface.ConnectionType.StrightLine, from, to);
        //            case UmlElementType.typeTransition:
        //                return new UmlTransitionModel(type, from, to);
        //            default:
        //                return new Sketch.Models.ConnectorModel(type, from, to);
        //        }
        //    }

        //    public IList<IBoundedItemFactory> GetConnectableFactories()
        //    {
        //        switch(_selectConnectorState)
        //        {
        //            case UmlElementType.typeAssociation:
        //                return this._associationOrComposition;
        //            case UmlElementType.typeComposition:
        //                return this._associationOrComposition;
        //            case UmlElementType.typeGeneralisation:
        //                return this._inheritance;
        //            case UmlElementType.typeTransition:
        //                return _stateActionOrChoice;
        //            default:
        //                return this._dependency;

        //        }
        //    }

        //    public IList<ICommandDescriptor> Tools
        //    {
        //        get
        //        {
        //            return _tools;
        //        }
        //    }

        //    public IList<ICommandDescriptor> Palette
        //    {
        //        get
        //        {
        //            return _tools;
        //        }
        //    }

        //    public ICommand SelectClass
        //    {
        //        get { return _selectClass; }
        //    }

        //    public ICommand SelectPackage
        //    {
        //        get
        //        {
        //            return _selectPackage;
        //        }
        //    }

        //    public ICommand SelectState
        //    {
        //        get
        //        {
        //            return _selectState;
        //        }
        //    }

        //    public ICommand SelectChoice
        //    {
        //        get
        //        {
        //            return _selectChoice;
        //        }
        //    }

        //    public ICommand SelectLifeLine
        //    {
        //        get
        //        {
        //            return _selectLifeLine;
        //        }
        //    }

        //    public ICommand SelectNote
        //    {
        //        get
        //        {
        //            return _selectNote;
        //        }
        //    }

        //    public ICommand SelectFinalState
        //    {
        //        get
        //        {
        //            return _selectFinalState;
        //        }
        //    }

        //    public ICommand SelectInitialState
        //    {
        //        get
        //        {
        //            return _selectInitialState;
        //        }
        //    }

        //    public UmlElementType Selection
        //    {
        //        get { return this._selectItemState; }
        //    }

        //    public event EventHandler OnSelectElement;

        //    public event EventHandler OnSelectConnector;

        //    private void NotifyOnSelectElement( UmlElementType type )
        //    {
        //        _selectItemState = type;

        //        if( OnSelectElement != null)
        //        {
        //            OnSelectElement(this, null);
        //        }
        //    }

        //    private void NotifyOnSelectConnector( UmlElementType type)
        //    {
        //        _selectConnectorState = type;
        //        {
        //            if( OnSelectConnector != null)
        //            {
        //                OnSelectConnector(this, null);
        //            }
        //        }
        //    }

        //    private void CreateConnectorEndFactories()
        //    {
        //        _classFactory = new ConnectableBaseFactory()
        //        {
        //            Name = "New Class",
        //            ToolTip = "Create new class",
        //            Bitmap = Properties.Resources.UmlClassShape, 
        //            CreateConnectableItem = doCreateClass,
        //        };

        //        _stateFactory = new ConnectableBaseFactory()
        //        {
        //            Name = "New State",
        //            ToolTip = "Create new state",
        //            Bitmap = Properties.Resources.UmlStateShape,
        //            CreateConnectableItem = doCreateState,
        //        };

        //        _choiceFactory = new ConnectableBaseFactory()
        //        {
        //            Name = "New State",
        //            ToolTip = "Create new state",
        //            Bitmap = Properties.Resources.UmlChoiceShape,
        //            CreateConnectableItem = doCreateChoice,
        //        };

        //        _packageFactory = new ConnectableBaseFactory()
        //        {
        //            Name = "New Package",
        //            ToolTip = "Create new package",
        //            Bitmap = Properties.Resources.UmlPackageShape,
        //            CreateConnectableItem = doCreatePackage,
        //        };

        //        _noteFactory = new ConnectableBaseFactory()
        //        {
        //            Name = "New Note",
        //            ToolTip = "Create new note",
        //            Bitmap = Properties.Resources.UmlNoteShape,
        //            CreateConnectableItem = doCreateNote,
        //        };

        //        _finalStateFactory = new ConnectableBaseFactory()
        //        {
        //            Name = "Final-State",
        //            ToolTip = "Add final State",
        //            CreateConnectableItem = doCreateFinalState,
        //            Bitmap = Resources.UmlFinalStateShape
        //        };

        //        _initialStateFactory = new ConnectableBaseFactory()
        //        {
        //            Name = "Initial-State",
        //            ToolTip = "Add Init-State",
        //            CreateConnectableItem = doCreateInitialState,
        //            Bitmap = Resources.UmlInitialStateShape,
        //        };

        //        _inheritance = new List<IBoundedItemFactory>() { _classFactory };
        //        _associationOrComposition = new List<IBoundedItemFactory>() { _packageFactory, _classFactory };
        //        _transition = new List<IBoundedItemFactory>() { _stateFactory, _choiceFactory };
        //        _dependency = new List<IBoundedItemFactory>() { _packageFactory, _classFactory, _noteFactory };
        //        _stateActionOrChoice = new List<IBoundedItemFactory>() {
        //            _stateFactory, _finalStateFactory, _choiceFactory };
        //    }

        //    private Sketch.Models.ConnectableBase doCreateNote(Point p)
        //    {
        //        var model = new UmlNoteModel(
        //            p, null);
        //        return model;
        //    }

        //    private Sketch.Models.ConnectableBase doCreateFinalState(Point p)
        //    {
        //        var model = new UmlFinalStateModel(
        //            p, null);
        //        return model;
        //    }

        //    private Sketch.Models.ConnectableBase doCreateInitialState(Point p)
        //    {
        //        var model = new UmlInitialStateModel(
        //            p, _initialStateToolFactory);
        //        return model;
        //    }

        //    private Sketch.Models.ConnectableBase doCreateState(Point p)
        //    {
        //        var model = new UmlStateModel(
        //            p, _stateToolFactory);
        //        return model;
        //    }

        //    private Sketch.Models.ConnectableBase doCreateClass(Point p)
        //    {
        //        var model = new UmlClassModel(
        //            p, _classToolsFactory);
        //        return model;
        //    }

        //    private Sketch.Models.ConnectableBase doCreatePackage(Point p)
        //    {
        //        var model = new UmlPackageModel(p, _packageToolFactory);
        //        return model;
        //    }

        //    private Sketch.Models.ConnectableBase doCreateChoice(Point p)
        //    {
        //        var model = new UmlChoiceModel(p, _choiceToolFactory);
        //        return model;
        //    }

        //    private Sketch.Models.ConnectableBase doCreatePicture(Point p)
        //    {
        //        var dlg = new Microsoft.Win32.OpenFileDialog();
        //        if (dlg.ShowDialog() == true)
        //        {
        //            var model = new DrawIt.Shapes.PictureModel(p, dlg.FileName, _pictureToolFactory);
        //            return model;
        //        }
        //        return null;
        //    }

        SketchItemFactory _factory = new SketchItemFactory();

        public UmlShapeFactory()
        {
            _factory.RegisterSketchItem(typeof(UmlClassModel), "Class", "Add new class", Properties.Resources.UmlClassShape);
            _factory.RegisterSketchItem(typeof(UmlClassModel), "Package", "Add new package", Properties.Resources.UmlPackageShape);
            _factory.RegisterSketchItem(typeof(UmlStateModel), "State", "Add a new state", Properties.Resources.UmlStateShape);
            _factory.RegisterSketchItem(typeof(UmlChoiceModel), "Decision Point", "Add a new decision point", Properties.Resources.UmlChoiceShape);
            _factory.RegisterSketchItem(typeof(UmlInitialStateModel), "Init State", "Add a init state", Properties.Resources.UmlInitialStateShape);
            _factory.RegisterSketchItem(typeof(UmlFinalStateModel),"Final State", "Add a final state", Properties.Resources.UmlFinalStateShape);
            _factory.RegisterSketchItem(typeof(UmlTransitionModel), "Transition", "Add a transition", Properties.Resources.UmlAssociationShape);
            _factory.RegisterSketchItem(typeof(UmlAssociationModel), "Association", "Add a association", Properties.Resources.UmlAssociationShape);
            _factory.RegisterSketchItem(typeof(UmlCompositionModel), "Composition", "Add a composition", Properties.Resources.UmlCompositionShape);
            _factory.RegisterSketchItem(typeof(UmlDependencyModel), "Dependency", "Add a dependency", Properties.Resources.UmlDependencyShape);
            _factory.RegisterSketchItem(typeof(UmlNoteModel), "Note", "Add a note", Properties.Resources.UmlNoteShape);
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

        public IConnectorItemModel CreateConnector(Type cls, ConnectionType type, IBoundedItemModel from, IBoundedItemModel to)
        {
            return _factory.CreateConnector(cls, type, from, to);
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
