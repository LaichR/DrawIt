
using Sketch.Interface;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Sketch.Helper.Binding;
using System.Linq.Expressions;
using LinqExp = System.Linq.Expressions.Expression;
using System.Runtime.Hosting;
using System.Security.Cryptography;
using Sketch.Models;
using Bitmap = System.Drawing.Bitmap;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sketch.Helper.RuntimeCheck;

namespace Sketch.Models
{
    public class SketchItemGroup : ISketchItemGroup
    {
        readonly string _name;
        readonly SketchItemFactory _parent;
        readonly List<ICommandDescriptor> _paletteCommands = new List<ICommandDescriptor>();
        readonly List<Type> _types = new List<Type>();
        public SketchItemGroup(string name, SketchItemFactory parent)
        {
            _name = name;
            _parent = parent;
        }

        public void AddType(Type cls)
        {
            if (cls.GetInterface(nameof(IBoundedSketchItemModel)) != null)
            {
                if (_types.Contains(cls)) return;
                _types.Add(cls);
            }
        }

        public string Name => _name;

        public IList<ICommandDescriptor> Palette
        {
            get
            {
                if (_paletteCommands.Count() == 0)
                {
                    _paletteCommands.AddRange(
                        _parent.PaletteCommands.Where<KeyValuePair<Type, CommandDescriptor>>((x) => _types.Contains(x.Key)).
                        Select<KeyValuePair<Type, CommandDescriptor>, ICommandDescriptor>((y) => y.Value).OrderBy((x)=>x.Name));
                }
                return _paletteCommands;
            }
        }
    }
    public abstract class SketchItemFactory: ISketchItemFactory, INotifyPropertyChanged
    {
        
        

        static readonly Type[] boundedItemfactorOpParam = new Type[]
        {
            typeof(System.Windows.Point),
            typeof(ISketchItemContainer)
        };

        static readonly Type[] connectorItemfactorOpParam = new Type[]
        {
            typeof(ConnectionType),
            typeof(IBoundedSketchItemModel),
            typeof(IBoundedSketchItemModel),
            typeof(Point),
            typeof(Point),
            typeof(ISketchItemContainer)
        };

        Type _selectedType = null;
        Bitmap _selectedItemBitmap = null;

        readonly Dictionary<Type, ConnectionType> _connectionTypeDefault = new Dictionary<Type, ConnectionType>();
        readonly Dictionary<Type, CommandDescriptor> _paletteCommands = new Dictionary<Type, CommandDescriptor>();
        readonly Dictionary<string, ISketchItemGroup> _sketchItemGroups = new Dictionary<string, ISketchItemGroup>(); 
        readonly Dictionary<Type, CreateBoundedSketchItemDelegate> _createBoundedItem = new Dictionary<Type, CreateBoundedSketchItemDelegate>();
        readonly Dictionary<Type, CreateConnectorDelegate> _createConnectorItem = new Dictionary<Type, CreateConnectorDelegate>();
        readonly Dictionary<Type, List<Type>> _allowableConnectorTypes = new Dictionary<Type, List<Type>>();
        readonly Dictionary<Type, List<ICommandDescriptor>> _allowableConnectorCmdDesc = new Dictionary<Type, List<ICommandDescriptor>>();
        readonly Dictionary<Type, List<Type>> _allowableConnectorTargetTypes = new Dictionary<Type, List<Type>>();
        readonly Dictionary<Type, List<IBoundedItemFactory>> _allowableConnectorTargetTypesFactories = new Dictionary<Type, List<IBoundedItemFactory>>();
        readonly List<ISketchItemGroup> _itemGroups = new List<ISketchItemGroup>();
        readonly string _name;

        protected SketchItemFactory(string name)
        {
            _name = name;
            InitializeFactory();
        }

        public abstract void InitializeFactory();


        public string Name
        {
            get => _name;
        }

        internal Dictionary<Type, CommandDescriptor> PaletteCommands => _paletteCommands;

        internal static ISketchItemFactory ActiveFactory
        {
            get;
            set;
        }



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
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetInitialSelection(Type t)
        {
            Contract.Requires<ArgumentNullException>(t != null, "Initial selection must not be null");
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

        public Bitmap SelectedItemBitmap
        {
            get => _selectedItemBitmap;
            private set
            {
                SetProperty<Bitmap>(ref _selectedItemBitmap, value);
            }
        }

        public IList<ICommandDescriptor> Palette
            => new List<ICommandDescriptor>(_paletteCommands.Where(
                (x)=>x.Key.GetInterface(nameof(IBoundedSketchItemModel)) != null)
                    .Select((x)=>x.Value).OrderBy((x) => x.Name));

        public IList<ISketchItemGroup> ItemGroups
        {
            get => new List<ISketchItemGroup>(_sketchItemGroups.Values);
        }

        public IBoundedSketchItemModel CreateConnectableSketchItem(Type cls, System.Windows.Point p, ISketchItemContainer container)
        {

            if (_createBoundedItem.TryGetValue(cls, out CreateBoundedSketchItemDelegate factoryOp))
            {
                return factoryOp(p, container);
            }
            throw new KeyNotFoundException(string.Format("No factory operation registered for class {0}", cls.Name));
        }

        public IConnectorItemModel CreateConnector(Type cls,
            IBoundedSketchItemModel from, IBoundedSketchItemModel to, 
            Point startPointHint, Point endPointHint,
            ISketchItemContainer container)
        {
            if(!_connectionTypeDefault.TryGetValue(cls, out ConnectionType connectionType))
            {
                connectionType = ConnectionType.AutoRouting;
            }
            if( _createConnectorItem.TryGetValue(cls, out CreateConnectorDelegate factorOp))
            {
                return factorOp(connectionType, from, to, startPointHint, endPointHint, container);
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
                if (sketchItemType.GetInterface(nameof(IBoundedSketchItemModel)) != null)
                {
                    var factory = CreateFactoryOp<CreateBoundedSketchItemDelegate>(sketchItemType, boundedItemfactorOpParam);
                    _createBoundedItem[sketchItemType] = factory;
                    CreateAndRegisterAllowalbleConnectors(sketchItemType);
                }
                else if (sketchItemType.GetInterface(nameof(IConnectorItemModel)) != null)
                {
                    var factory = CreateFactoryOp<CreateConnectorDelegate>(sketchItemType, connectorItemfactorOpParam);
                    _createConnectorItem[sketchItemType] = factory;
                    CreateAndRegisterAllowalbleConnectionEnds(sketchItemType);
                }
            }
        }

        public void RegisterSketchItemInCategory(string category, 
            Type sketchItemType, string menuLabel, string menuBrief, Bitmap toolsBitmap)
        {
            RegisterSketchItem(sketchItemType, menuLabel, menuBrief, toolsBitmap);
            if( !_sketchItemGroups.TryGetValue(category, out ISketchItemGroup group))
            {
                group = new SketchItemGroup(category, this);
                _sketchItemGroups.Add(category, group);
            }
            group.AddType(sketchItemType);
        }

        CommandDescriptor CreatePaletteCommandDescriptor(Type type, string menuLabel, string menuBrief, Bitmap toolsBitmap)
        {
            var cmdDescriptor = new CommandDescriptor()
            {
                Bitmap = toolsBitmap,
                Name = menuLabel,
                ToolTip = menuBrief,
                Command = new DelegateCommand(
                    () => { SelectedForCreation = type; SelectedItemBitmap = toolsBitmap; })
            };
            return cmdDescriptor;
        }

        T CreateFactoryOp<T>(Type cls, Type[] paramTypes ) where T: Delegate
        {
            var ctor = cls.GetConstructor(paramTypes);
            Assert.True(ctor != null, "no matching constructor found");

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
            OnBoundedItemSelected?.Invoke(_selectedType, EventArgs.Empty);
        }

        
        void NotifyOnConnectorItemSelected()
        {
             OnConnectorItemSelected?.Invoke(_selectedType, EventArgs.Empty);   
        }

        void SetProperty<T>(ref T propertyValue, T value, [CallerMemberName] string name = "")
        {
            propertyValue = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
