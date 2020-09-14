using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Sketch.Types;
using Prism.Commands;
using UI.Utilities.Interfaces;
using UI.Utilities.Behaviors;
using Sketch.Interface;

namespace Sketch.Models
{
    public class ConnectorModel: ModelBase, IConnectorItemModel
    {
        static readonly double HotSpotSize = 8;
        static readonly double LabelDistanceY = 15;
        static readonly double LabelDistanceX = 15;
        static readonly double LabelDefaultHeight = 20;
        static readonly double LabelDefaultWidht = 75;

        static readonly string ShowLabelHeader = "Show Label";
        static readonly string HideLabelHeader = "Hide Label";
        static readonly string ReverseDirectionHeader = "Reverse direction";
        static readonly string UpdateFromHeader = "Update connection start";
        static readonly string UpdateToHeader = "Update connection end";

        DoubleCollection _dashArray;
        IConnectorStrategy _myConnectorStrategy;

        
        PathGeometry _path = new PathGeometry();
        GeometryGroup _geometry = new GeometryGroup();

        [PersistentField(ModelVersion.V_0_1, "ConnectionType")]
        ConnectionType _connectionType;
        [PersistentField(ModelVersion.V_0_1, "StartPointDocking")]
        ConnectorDocking _startpointDocking;
        [PersistentField(ModelVersion.V_0_1, "EndPointDocking")]
        ConnectorDocking _endpointDocking;

        [PersistentField(ModelVersion.V_0_1, "StartPointRelativePosition")]
        double _startPointRelativePosition;
        [PersistentField(ModelVersion.V_0_1, "EndPointRelativePosition")]
        double _endPointRelativePosition;
        [PersistentField(ModelVersion.V_0_1, "MiddlePointRelativePosition")]
        double _middlePointRelativePosition;

        [PersistentField(ModelVersion.V_0_1, "IsLabelShown")]
        bool _isLabelShown = false;
        

        [PersistentField(ModelVersion.V_0_1, "Container")]
        SketchItemContainerProxy _containerProxy;
        ISketchItemContainer _container;          // if i want to prevent two overlaying connections, 
                                                  // i need to have some knowledge about other connections

        [PersistentField(ModelVersion.V_0_1, "From")]
        ConnectableBase _from;

        [PersistentField(ModelVersion.V_0_1, "To")]
        ConnectableBase _to;

        string _showHideLabelHeader = ShowLabelHeader;
        bool _isRewireing = false;

        
        ConnectorLabelModel _connectorStartLabel;

        
        DelegateCommand _cmdShowLabel;
        DelegateCommand _cmdHideLabel;
        DelegateCommand _cmdReverseDirection;
        DelegateCommand _cmdUpdateFrom;
        DelegateCommand _cmdUpdateTo;

        
        IBoundedItemModel _backup;
        Action _restoreAction;


        public ConnectorModel(ConnectionType type,
            IBoundedItemModel from, IBoundedItemModel to,
            ISketchItemContainer container)
        //:base(new Guid())
        {
            _container = container;
            _connectionType = type;
            MiddlePointRelativePosition = 0.5;
            From = from;
            To = to;
            _myConnectorStrategy = ConnectorUtilities.GetConnectionType(this, _connectionType);
            Label = string.Format("{0}->{1}", from.Label, to.Label);
            LineWidth = 1;
            _geometry.Children.Add(_path);
            Initialize();
        }

        protected override void RestoreFieldData(SerializationInfo info, StreamingContext context)
        {
            base.RestoreFieldData(info, context);
        }

        protected override void Initialize()
        {
            _cmdShowLabel = new DelegateCommand(ExecuteShowLabel, CanExecuteShowLabel);
            _cmdHideLabel = new DelegateCommand(ExecuteHideLabel, CanExecuteHideLabel);
            _cmdReverseDirection = new DelegateCommand(ExecuteChangeDirection);
            _cmdUpdateFrom = new DelegateCommand(ExecuteChangeFrom);
            _cmdUpdateTo = new DelegateCommand(ExecuteChangeTo);
            if (_isLabelShown)
            {
                ExecuteShowLabel();
            }
            UpdateGeometry();
        }

        public virtual IList<IBoundedItemFactory> AllowableConnectorTargets
        {
            get => SketchItemFactory.ActiveFactory?.GetConnectableFactories(this.GetType());
        }

        protected ConnectorModel(SerializationInfo info, StreamingContext context)
            : base(info, context){} // initialize is called in Model Base

        public override ISketchItemModel RefModel
        {
            get
            {
                return _connectorStartLabel;
            }
        }

        public bool FillEndings
        {
            get;
            set;
        }

        public bool IsRewireing
        {
            get
            {
                return _isRewireing;
            }
        }

        public int LineWidth    
        {
            get;
            set;
        }

        public DoubleCollection StrokeDashArray
        {
            get { return _dashArray; }
            protected set { _dashArray = value; }
        }



        public override Geometry Geometry
        {
            get
            {
                return _geometry;
            }
        }

        public IConnectorMoveHelper StartMove(  Point p)
        {
            return _myConnectorStrategy.StartMove( p);
        }

        public Point ConnectorStart
        {
            get { return _myConnectorStrategy.ConnectionStart; }
        }

        public Point ConnectorEnd
        {
            get { return _myConnectorStrategy.ConnectionEnd; }
        }

        public Rect HotSpotEnd
        {
            get
            {
                return GetHotSpotRectangle(ConnectorEnd);
            }
        }

        public Rect HotSpotStart
        {
            get
            {
                return GetHotSpotRectangle(ConnectorStart);
            }
        }

        public bool ConnectorStartSelected
        {
            get;
            set;
        }

        public bool ConnectorEndSelected
        {
            get;
            set;
        }

        public ConnectorLabelModel ConnectorStartLabel
        {
            get
            {
                return _connectorStartLabel;
            }
            private set
            {
                SetProperty<ConnectorLabelModel>(ref _connectorStartLabel, value );
            }
        }

        public IEnumerable<ConnectorModel> SiblingConnections => 
            _container?.SketchItems.OfType<ConnectorModel>().Where((x)=> x != this);

        public IEnumerable<ConnectableBase> Connectables =>
            _container?.SketchItems.OfType<ConnectableBase>();

        public IList<ICommandDescriptor> ContextMenuDeclaration
        {
            get
            {
                var showLabel = new CommandDescriptor
                {
                    Name = ShowLabelHeader,
                    Command = _cmdShowLabel
                };

                var hideLabel = new CommandDescriptor
                {
                    Name = HideLabelHeader,
                    Command = _cmdHideLabel
                };

                var reverseDirection = new CommandDescriptor
                {
                    Name = ReverseDirectionHeader,
                    Command = _cmdReverseDirection
                };

                var updateFrom = new CommandDescriptor
                {
                    Name = UpdateFromHeader,
                    Command = _cmdUpdateFrom
                };

                var updateTo = new CommandDescriptor
                {
                    Name = UpdateToHeader,
                    Command = _cmdUpdateTo
                };

                return new List<ICommandDescriptor>
                {
                    showLabel,
                    hideLabel,
                    reverseDirection,
                    updateTo,
                    updateFrom
                };
            }
        }


        public override void Move(Transform translation)
        {
            if( ConnectorStartSelected )
            {
                var helper = ConnectorStrategy.StartMove(ConnectorStart);
                var p = translation.Transform(ConnectorStart);
                p = ConnectorUtilities.RestrictRange(From.Bounds,p);
                helper.Commit(StartPointDocking, EndPointDocking, p, ConnectorEnd, helper.Distance);
            }
            if( ConnectorEndSelected)
            {
                var helper = ConnectorStrategy.StartMove(ConnectorEnd);
                var p = translation.Transform(ConnectorEnd);
                p = ConnectorUtilities.RestrictRange(To.Bounds, p);
                helper.Commit(EndPointDocking, StartPointDocking, p, ConnectorStart, helper.Distance);
            }
            UpdateGeometry();
        }

        public IBoundedItemModel From
        {
            get { return _from; }
            set { SetProperty<ConnectableBase>(ref _from, value as ConnectableBase); }
        }

        public IBoundedItemModel To
        {
            get { return _to; }
            set { SetProperty<ConnectableBase>(ref _to, value as ConnectableBase); }
        }

        public ConnectorDocking StartPointDocking
        {
            get { return _startpointDocking; }
            set { _startpointDocking = value; }
        }
    
        public ConnectorDocking EndPointDocking
        {
            get { return _endpointDocking; }
            set { _endpointDocking = value; }
        }

        public double StartPointRelativePosition
        {
            get => _startPointRelativePosition;
            set => _startPointRelativePosition = value;
        }

        public double EndPointRelativePosition
        {
            get => _endPointRelativePosition;
            set => _endPointRelativePosition = value;
        }

        public double MiddlePointRelativePosition
        {
            get => _middlePointRelativePosition;
            set => _middlePointRelativePosition = value;
        }

        public IConnectorStrategy ConnectorStrategy
        {
            get
            {
                return _myConnectorStrategy;
            }
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        protected override void PrepareFieldBackup()
        {
            base.PrepareFieldBackup();
            _containerProxy = new SketchItemContainerProxy(this._container);
        }

        protected override void FieldDataRestored()
        {
            base.FieldDataRestored();
            _container = _containerProxy.Container;
            _myConnectorStrategy = ConnectorUtilities.GetConnectionType(this, _connectionType);

        }


        public void ShowLabelConnectorStart( Point p)
        {
            if (ConnectorStartLabel == null)
            {
                var labelModel = new ConnectorLabelModel(this, true, p);
                ConnectorStartLabel = labelModel;
                _cmdHideLabel.RaiseCanExecuteChanged();
                _cmdShowLabel.RaiseCanExecuteChanged();
                _isLabelShown = true;
            }
        }

        public void RestoreConnectionEnd()
        {
            _restoreAction();
        }

        public override void UpdateGeometry()
        {
            // this might be triggered before the full object is available
            // hence we need to prevent this from doing bad stuff!
            if (_geometry != null && _myConnectorStrategy != null)
            {
                _geometry.Children.Clear();
                _path.Figures = new PathFigureCollection(_myConnectorStrategy.ConnectorPath);
                _geometry.Children.Add(_path);
                ConnectorStartLabel?.UpdateGeometry();
            }
        }

        Point GetLabelPosition()
        {
            if (LabelArea.Width == 0)
            {
                switch (StartPointDocking)
                {
                    case ConnectorDocking.Top:
                        {
                            double y = ConnectorStart.Y - LabelDistanceY - LabelDefaultHeight;
                            double x = ConnectorStart.X + LabelDistanceX;
                            if (ConnectorEnd.X > ConnectorStart.X)
                            {
                                x = ConnectorStart.X - LabelDistanceX - LabelDefaultWidht;
                            }
                            return new Point(x, y);
                        }
                    case ConnectorDocking.Left:
                        {
                            double y = ConnectorStart.Y - LabelDistanceY - LabelDefaultHeight;
                            double x = ConnectorStart.X - LabelDistanceX - LabelDefaultWidht;

                            if (ConnectorStart.Y < ConnectorEnd.Y)
                            {
                                y = ConnectorStart.Y + LabelDistanceY;
                            }
                            return new Point(x, y);
                        }
                    case ConnectorDocking.Bottom:
                        {
                            double y = ConnectorStart.Y + LabelDistanceY;
                            double x = ConnectorStart.X + LabelDistanceX;
                            if (ConnectorEnd.X > ConnectorStart.X)
                            {
                                x = ConnectorStart.X - LabelDistanceX - LabelDefaultWidht;
                            }
                            return new Point(x, y);
                        }

                    case ConnectorDocking.Right:
                        {
                            double y = ConnectorStart.Y - LabelDistanceY - LabelDefaultHeight;
                            double x = ConnectorStart.X + LabelDistanceX;
                            if (ConnectorStart.Y < ConnectorEnd.Y)
                            {
                                y = ConnectorStart.Y + LabelDistanceY;
                            }
                            return new Point(x, y);
                        }


                    default:
                        throw new NotSupportedException("the start point should have a docking");

                }
            }
            return LabelArea.Location;
        }

        Rect GetHotSpotRectangle(Point p)
        {
            var rect = new Rect(new Point(p.X - HotSpotSize / 2, p.Y - HotSpotSize / 2),
                    new Size(HotSpotSize, HotSpotSize));
            return rect;
        }

        void ExecuteShowLabel()
        {
            ShowLabelConnectorStart(GetLabelPosition());
        }

        bool CanExecuteShowLabel()
        {
            var ret = _connectorStartLabel == null;

            return ret;
        }

        void ExecuteHideLabel()
        {
            ConnectorStartLabel = null;
            _isLabelShown = false;
            _cmdHideLabel.RaiseCanExecuteChanged();
            _cmdShowLabel.RaiseCanExecuteChanged();
        }

        bool CanExecuteHideLabel()
        {
            return _connectorStartLabel != null;
        }


        void ExecuteChangeDirection()
        {
            var tmpFrom = From;
            _isRewireing = true;
            From = To;
            To = tmpFrom;
            _isRewireing = false;
        }

        void ExecuteChangeFrom()
        {
            _isRewireing = true;
            _backup = From;
            _restoreAction = () => { From = _backup; };
            From = null;
            _isRewireing = false;
        }

        void ExecuteChangeTo()
        {
            _isRewireing = true;
            _backup = To;
            _restoreAction = () => { To = _backup; };
            To = null;
            _isRewireing = false;
        }

    }
}
