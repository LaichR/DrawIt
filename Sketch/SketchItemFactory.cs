using Prism.Commands;
using Sketch.Interface;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UI.Utilities.Behaviors;
using UI.Utilities.Interfaces;
using System.Linq.Expressions;
using LinqExp = System.Linq.Expressions.Expression;
using System.Runtime.Hosting;
using System.Security.Cryptography;
using Sketch.Models;
using System.Drawing;

namespace Sketch
{
    public class SketchItemFactory: ISketchItemFactory
    {
        static readonly Type[] boundedItemfactorOpParam = new Type[]
        {
            typeof(System.Windows.Point)
        };

        static readonly Type[] connectorItemfactorOpParam = new Type[]
        {
            typeof(ConnectionType),
            typeof(IBoundedItemModel),
            typeof(IBoundedItemModel)
        };

        Type _selectedType = null;

        Dictionary<Type, CommandDescriptor> _paletteCommands = new Dictionary<Type, CommandDescriptor>();
        Dictionary<Type, CreateBoundedSketchItemDelegate> _createBoundedItem = new Dictionary<Type, CreateBoundedSketchItemDelegate>();
        Dictionary<Type, CreateConnectorDelegate> _createConnectorItem = new Dictionary<Type, CreateConnectorDelegate>();
        Dictionary<Type, List<Type>> _allowableConnectorTypes = new Dictionary<Type, List<Type>>();
        Dictionary<Type, List<ICommandDescriptor>> _allowableConnectorCmdDesc = new Dictionary<Type, List<ICommandDescriptor>>();
        Dictionary<Type, List<Type>> _allowableConnectorTargetTypes = new Dictionary<Type, List<Type>>();
        Dictionary<Type, List<IBoundedItemFactory>> _allowableConnectorTargetTypesFactories = new Dictionary<Type, List<IBoundedItemFactory>>();



        public void RegisterBoundedItemSelectedNotification(EventHandler handler)
        {
            OnBoundedItemSelected += handler;
        }
        public void UnregisterBoundedItemSelectedNotification(EventHandler handler)
        {
            OnBoundedItemSelected -= handler;
        }
        public void RegisterConnectorItemSelectedNotification(EventHandler handler)
        {
            OnConnectorItemSelected += handler;
        }

        public void UnregisterConnectorItemSelectedNotification(EventHandler handler)
        {
            OnConnectorItemSelected -= handler;
        }

        event EventHandler OnBoundedItemSelected;

        event EventHandler OnConnectorItemSelected;

        public void SetInitialSelection(Type t)
        {
            RuntimeCheck.Contract.Requires<ArgumentNullException>(t != null, "Initial selection must not be null");
            _selectedType = t;
        }

        public Type SelectedForCreation
        {
            get => _selectedType;
            private set
            {
                _selectedType = value;
                if (value.IsSubclassOf(typeof(ConnectableBase)))
                {
                    NotifyOnBoundedItemSelected();
                }
                else if( value.IsSubclassOf(typeof(ConnectorModel)))
                {
                    NotifyOnConnectorItemSelected();
                }
            }
        }

        public IList<ICommandDescriptor> Palette
            => new List<ICommandDescriptor>(_paletteCommands.Where(
                (x)=>x.Key.GetInterface("IBoundedItemModel") != null)
                .Select((x)=>x.Value).OrderBy((x) => x.Name));

        public IBoundedItemModel CreateConnectableSketchItem(Type cls, System.Windows.Point p)
        {
            if (_createBoundedItem.TryGetValue(cls, out CreateBoundedSketchItemDelegate factoryOp))
            {
                return factoryOp(p);
            }
            throw new KeyNotFoundException(string.Format("No factory operation registered for class {0}", cls.Name));
        }

        public IConnectorItemModel CreateConnector(Type cls, ConnectionType type, IBoundedItemModel from, IBoundedItemModel to)
        {
            if( _createConnectorItem.TryGetValue(cls, out CreateConnectorDelegate factorOp))
            {
                return factorOp(type, from, to);
            }
            throw new KeyNotFoundException(string.Format("No factory operation registered for class {0}", cls.Name));
        }

        public IList<IBoundedItemFactory> GetConnectableFactories(Type cls)
        {
            
            if (!_allowableConnectorTargetTypesFactories.TryGetValue(cls, out List<IBoundedItemFactory> factoryList))
            {
                factoryList = new List<IBoundedItemFactory>();
                if (_allowableConnectorTargetTypes.TryGetValue(cls, out List<Type> connectorTargetTypes))
                {
                    foreach (var t in connectorTargetTypes)
                    {
                        if (_createBoundedItem.TryGetValue(t, out CreateBoundedSketchItemDelegate factoryOp) &&
                            _paletteCommands.TryGetValue(t, out CommandDescriptor cmdDesc))
                        {
                            var factory = new ConnectableBaseFactory()
                            {
                                Bitmap = cmdDesc.Bitmap,
                                ToolTip = cmdDesc.ToolTip,
                                Name = cmdDesc.Name,
                                CreateConnectableItem = factoryOp
                            };
                            factoryList.Add(factory);
                        }
                    }
                }
                _allowableConnectorTargetTypesFactories[cls] = factoryList;
            }
            return factoryList;
        }

        public IList<ICommandDescriptor> GetAllowableConnctors(Type cls) 
        {
            if (!_allowableConnectorCmdDesc.TryGetValue(cls, out List<ICommandDescriptor> l))
            {
                l = new List<ICommandDescriptor>();
                if (_allowableConnectorTypes.TryGetValue(cls, out List<Type> connectorTypes))
                {
                    l.AddRange(new List<ICommandDescriptor>(_paletteCommands.Where((x) =>
                       connectorTypes.Contains(x.Key)).
                            Select<KeyValuePair<Type, CommandDescriptor>, ICommandDescriptor>((x) => x.Value)));
                }
                _allowableConnectorCmdDesc.Add(cls, l);
            }
            return l;
        }

        /// <summary>
        /// Allows to register a type
        /// </summary>
        /// <param name="sketchItemType"></param>
        public void RegisterSketchItem( Type sketchItemType, string menuLabel, string menuBrief, Bitmap toolsBitmap )
        {
            if (!_paletteCommands.TryGetValue(sketchItemType, out CommandDescriptor commandDescriptor))
            {
                commandDescriptor = CreatePaletteCommandDescriptor(sketchItemType, menuLabel, menuBrief, toolsBitmap);
                _paletteCommands[sketchItemType] = commandDescriptor;
                if (sketchItemType.GetInterface("IBoundedItemModel") != null)
                {
                    var factory = CreateFactoryOp<CreateBoundedSketchItemDelegate>(sketchItemType, boundedItemfactorOpParam);
                    _createBoundedItem[sketchItemType] = factory;
                    CreateAndRegisterAllowalbleConnectors(sketchItemType);
                }
                else if (sketchItemType.GetInterface("IConnectorItemModel") != null)
                {
                    var factory = CreateFactoryOp<CreateConnectorDelegate>(sketchItemType, connectorItemfactorOpParam);
                    _createConnectorItem[sketchItemType] = factory;
                    CreateAndRegisterAllowalbleConnectionEnds(sketchItemType);
                }
            }
        }

        CommandDescriptor CreatePaletteCommandDescriptor(Type type, string menuLabel, string menuBrief, Bitmap toolsBitmap)
        {
            var cmdDescriptor = new CommandDescriptor()
            {
                Bitmap = toolsBitmap,
                Name = menuLabel,
                ToolTip = menuBrief,
                Command = new DelegateCommand(
                    () => SelectedForCreation = type)
            };
            return cmdDescriptor;
        }

        T CreateFactoryOp<T>(Type cls, Type[] paramTypes ) where T: Delegate
        {
            var ctor = cls.GetConstructor(paramTypes);
            RuntimeCheck.Assert.True(ctor != null, "no matching constructor found");

            List<ParameterExpression> arguments = new List<ParameterExpression>
                (paramTypes.Select<Type, ParameterExpression>((x) =>
                    LinqExp.Parameter(x)));
            
            var fun = LinqExp
                .Lambda<T>(
                    LinqExp.New(ctor, arguments), arguments).Compile();
            return fun;
        }


        void CreateAndRegisterAllowalbleConnectors(Type cls)
        {
            var connectorTypes = cls.GetCustomAttributes(typeof(AllowableConnectorAttribute), true)
                .OfType<AllowableConnectorAttribute>()
                    .Select<AllowableConnectorAttribute, Type>((x) => x.AllowableConnectorType);
            _allowableConnectorTypes[cls] = new List<Type>(connectorTypes);
        }

        void CreateAndRegisterAllowalbleConnectionEnds(Type cls)
        {
            var connectorTargetTypes = cls.GetCustomAttributes(typeof(AllowableConnectorTargetAttribute), true)
                .OfType<AllowableConnectorTargetAttribute>()
                    .Select<AllowableConnectorTargetAttribute, Type>((x) => x.AllowableConnectorType);
            _allowableConnectorTargetTypes[cls] = new List<Type>(connectorTargetTypes);
        }

        void NotifyOnBoundedItemSelected()
        {
            if( OnBoundedItemSelected != null )
            {
                OnBoundedItemSelected(_selectedType, EventArgs.Empty);
            }
        }

        
        void NotifyOnConnectorItemSelected()
        {
            if( OnConnectorItemSelected != null)
            {
                OnConnectorItemSelected(_selectedType, EventArgs.Empty);
            }
        }
    }
}
