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
        
        ConnectionType _connectionType;
        PathGeometry _path = new PathGeometry();
        GeometryGroup _geometry = new GeometryGroup();
        ConnectorDocking _startpointDocking;
        ConnectorDocking _endpointDocking;
        string _showHideLabelHeader = ShowLabelHeader;
        bool _isRewireing = false;

        ConnectorLabelModel _connectorStartLabel;
        List<ConnectorModel> _siblingConnections = new List<ConnectorModel>(); // if i want to prevent two overlaying connections, 
                                                  // i need to have some knowledge about other connections

        DelegateCommand _cmdShowLabel;
        DelegateCommand _cmdHideLabel;
        DelegateCommand _cmdReverseDirection;
        DelegateCommand _cmdUpdateFrom;
        DelegateCommand _cmdUpdateTo;

        ConnectableBase _from;
        ConnectableBase _to;
        IBoundedItemModel _backup;
        Action _restoreAction;


        public ConnectorModel( ConnectionType type,
            IBoundedItemModel from, IBoundedItemModel to)
        //:base(new Guid())
        {
            _connectionType = type;
            MiddlePointRelativePosition = 0.5;
            From = from;
            To = to;
            _myConnectorStrategy = ConnectorUtilities.GetConnectionType(this, _connectionType);
            Name = string.Format("{0}->{1}", from.Name, to.Name);
            LineWidth = 1;
            _cmdShowLabel = new DelegateCommand(ExecuteShowLabel, CanExecuteShowLabel);
            _cmdHideLabel = new DelegateCommand(ExecuteHideLabel, CanExecuteHideLabel);
            _cmdReverseDirection = new DelegateCommand(ExecuteChangeDirection);
            _cmdUpdateFrom = new DelegateCommand(ExecuteChangeFrom);
            _cmdUpdateTo = new DelegateCommand(ExecuteChangeTo);
            _geometry.Children.Add(_path);
        }

        public virtual IList<IBoundedItemFactory> AllowableConnectorTargets
        {
            get => ModelFactoryRegistry.Instance.GetSketchItemFactory().GetConnectableFactories(this.GetType());
        }

        protected ConnectorModel(SerializationInfo info, StreamingContext context):base(info, context)
        {
            _connectionType = (ConnectionType)info.GetValue("ConnectionType", typeof(ConnectionType));
            StartPointRelativePosition = info.GetDouble("StartPointRelativePosition");
            MiddlePointRelativePosition = info.GetDouble("MiddlePointRelativePosition");
            EndPointRelativePosition = info.GetDouble("EndPointRelativePosition");
            EndPointDocking = (ConnectorDocking)info.GetValue("EndPointDocking", typeof(ConnectorDocking));
            StartPointDocking = (ConnectorDocking)info.GetValue("StartPointDocking", typeof(ConnectorDocking));
            var isLableVisible = info.GetBoolean("IsLabelShown");
            From = (ConnectableBase) info.GetValue("From", typeof(ConnectableBase));
            To = (ConnectableBase)info.GetValue("To", typeof(ConnectableBase));            
            _myConnectorStrategy = ConnectorUtilities.GetConnectionType(this, _connectionType);
            _cmdShowLabel = new DelegateCommand(ExecuteShowLabel, CanExecuteShowLabel);
            _cmdHideLabel = new DelegateCommand(ExecuteHideLabel, CanExecuteHideLabel);
            // this needs to be executed in the end, since it accesses _cmdShowLabel
            if (isLableVisible)
            {
                ExecuteShowLabel();
            }
        }

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

        internal void SetSiblings(IEnumerable<ConnectorModel> siblings)
        {
            RuntimeCheck.Contract.Requires<ArgumentNullException>(siblings != null, "siblings must not be null");
            _siblingConnections.Clear();
            _siblingConnections.AddRange(siblings);
        }

        public IEnumerable<ConnectorModel> Siblings => _siblingConnections;

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
                
                TriggerGeometryUpdate();
            }
            if( ConnectorEndSelected)
            {
                var helper = ConnectorStrategy.StartMove(ConnectorEnd);
                var p = translation.Transform(ConnectorEnd);
                p = ConnectorUtilities.RestrictRange(To.Bounds, p);
                helper.Commit(EndPointDocking, StartPointDocking, p, ConnectorStart, helper.Distance);
            }
            RaisePropertyChanged("Geometry");
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
            get;
            set;
        }

        public double EndPointRelativePosition
        {
            get;
            set;
        }

        public double MiddlePointRelativePosition
        {
            get;
            set;
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
            info.AddValue("ConnectionType", _connectionType);
            info.AddValue("StartPointRelativePosition", StartPointRelativePosition);
            info.AddValue("MiddlePointRelativePosition", MiddlePointRelativePosition);
            info.AddValue("EndPointRelativePosition", EndPointRelativePosition);
            info.AddValue("EndPointDocking", EndPointDocking);
            info.AddValue("StartPointDocking", StartPointDocking);
            info.AddValue("IsLabelShown", _connectorStartLabel != null);
            info.AddValue("From", From);
            info.AddValue("To", To);
            
        }

        public virtual void TriggerGeometryUpdate()
        {
            ComputeGeometry();
            
            if (_connectorStartLabel != null) _connectorStartLabel.InvalidateGeometry();
        }

        public void ShowLabelConnectorStart( Point p)
        {
            var labelModel = new ConnectorLabelModel(this, true, p);
            ConnectorStartLabel = labelModel;
            _cmdHideLabel.RaiseCanExecuteChanged();
            _cmdShowLabel.RaiseCanExecuteChanged();
        }

        public void RestoreConnectionEnd()
        {
            _restoreAction();
        }

        protected virtual void ComputeGeometry()
        {
            _geometry.Children.Clear();
            _path.Figures = new PathFigureCollection(_myConnectorStrategy.ConnectorPath);
            _geometry.Children.Add(_path);
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
