using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.ComponentModel;
using System.Collections;
using System.Windows.Navigation;
using Sketch.PropertyEditor;

namespace Sketch.Models
{
    public class ConnectorModel: ModelBase, IConnectorItemModel, IEnumerator<IWaypoint>
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
        bool _connectorStartSelected;
        bool _connectorEndSelected;
        int _waypointIndex;

        public static readonly System.Windows.Media.Pen HittestPen = new System.Windows.Media.Pen()
        {
            Brush = System.Windows.Media.Brushes.Transparent,
            Thickness = ComputeConnectorLine.LineWidth,
        };

        readonly PathGeometry _path = new PathGeometry();
        readonly GeometryGroup _geometry = new GeometryGroup();

        [PersistentField((int)ModelVersion.V_0_1, "ConnectionType")]
        readonly ConnectionType _connectionType;
        [PersistentField((int)ModelVersion.V_0_1, "StartPointDocking")]
        ConnectorDocking _startpointDocking;
        [PersistentField((int)ModelVersion.V_0_1, "EndPointDocking")]
        ConnectorDocking _endpointDocking;

        [PersistentField((int)ModelVersion.V_0_1, "StartPointRelativePosition")]
        double _startPointRelativePosition;


        [PersistentField((int)ModelVersion.V_0_1, "EndPointRelativePosition")]
        double _endPointRelativePosition;
        
        [PersistentField((int)ModelVersion.V_0_1, "MiddlePointRelativePosition")]
        double _middlePointRelativePosition;

        [PersistentField((int)ModelVersion.V_0_1, "IsLabelShown")]
        bool _isLabelShown = false;

        [PersistentField((int)ModelVersion.V_2_0, "IsEndpointLabelShown", true)]
        bool _isEndpointLabelShown = false;

        [PersistentField((int)ModelVersion.V_2_0, "EndpointLabel", true)]
        string _endPointLabel = "Endpoint";

        [PersistentField((int)ModelVersion.V_2_0, "EndpointLabelArea")]
        Rect _endPointLabelArea;

        [PersistentField((int)ModelVersion.V_2_1, "Waypoints")]
        readonly ObservableCollection<IWaypoint> _waypoints = new ObservableCollection<IWaypoint> ();

        [PersistentField((int)ModelVersion.V_0_1, "Container")]
        SketchItemContainerProxy _containerProxy;
        ISketchItemContainer _container;          // if i want to prevent two overlaying connections, 
                                                  // i need to have some knowledge about other connections

        [PersistentField((int)ModelVersion.V_0_1, "From")]
        ConnectableBase _from;

        [PersistentField((int)ModelVersion.V_0_1, "To")]
        ConnectableBase _to;

        
        bool _isRewireing = false;

        
        ConnectorLabelModel _connectorStartLabel;
        ConnectorLabelModel _connectorEndLabel;

        
        DelegateCommand _cmdShowLabel;
        DelegateCommand _cmdHideLabel;
        DelegateCommand _cmdReverseDirection;
        DelegateCommand _cmdUpdateFrom;
        DelegateCommand _cmdUpdateTo;

        
        IBoundedItemModel _backup;
        Action _restoreAction;


        public ConnectorModel(ConnectionType type,
            IBoundedItemModel from, IBoundedItemModel to,
            System.Windows.Point connectorStartHint, System.Windows.Point connectorEndHint,
            ISketchItemContainer container)
        //:base(new Guid())
        {
            _container = container;
            _connectionType = type;
            MiddlePointRelativePosition = 0.5;
            From = from;
            To = to;
            _myConnectorStrategy = ConnectorUtilities.GetConnectionType(this, _connectionType, connectorStartHint, connectorEndHint);
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
                ShowConnectorLabel(GetLabelPosition(true), true);
            }
            if( _isEndpointLabelShown )
            {
                ShowConnectorLabel(GetLabelPosition(false), false);
            }
            UpdateGeometry();
        }

        public bool AllowWaypoints
        {
            get => ConnectorStrategy.AllowWaypoints;
        }

        public ObservableCollection<IWaypoint> Waypoints => _waypoints;
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

        public IConnectorMoveHelper StartMove(  System.Windows.Point p)
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
            get => _connectorStartSelected;
            set
            {
                SetProperty<bool>(ref _connectorStartSelected, value);
                _cmdShowLabel.RaiseCanExecuteChanged();
            }
        }

        public bool ConnectorEndSelected
        {
            get => _connectorEndSelected;
            set
            {
                SetProperty<bool>(ref _connectorEndSelected, value);
                _cmdShowLabel.RaiseCanExecuteChanged();
            }
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
                _cmdHideLabel.RaiseCanExecuteChanged();
                _cmdShowLabel.RaiseCanExecuteChanged();
            }
        }

        public ConnectorLabelModel ConnectorEndLabel
        {
            get
            {
                return _connectorEndLabel;
            }
            private set
            {
                SetProperty<ConnectorLabelModel>(ref _connectorEndLabel, value);
                _cmdHideLabel.RaiseCanExecuteChanged();
                _cmdShowLabel.RaiseCanExecuteChanged();
            }
        }

        public string GetLabel(bool isStartPointLabel)
        {
            if( isStartPointLabel)
            {
                return Label;
            }
            return _endPointLabel;
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

        [Browsable(true)]
        public string EndpointLabel
        {
            get => _endPointLabel;
            set
            {
                SetProperty<string>(ref _endPointLabel, value);
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
            Point pStart = ConnectorUtilities.ComputePoint(From.Bounds, StartPointDocking, StartPointRelativePosition);
            Point pEnd = ConnectorUtilities.ComputePoint(To.Bounds, EndPointDocking, EndPointRelativePosition);
            _myConnectorStrategy = ConnectorUtilities.GetConnectionType(this, _connectionType, pStart, pEnd);
        }


        public virtual void ShowConnectorLabel( Point p, bool isConnectorStartLabel)
        {
           
           
            if (isConnectorStartLabel)
            {
                if (ConnectorStartLabel == null)
                {
                    var labelModel = new ConnectorLabelModel(this, true, ShowLableHaderConnection, p);
                    ConnectorStartLabel = labelModel;
                    _isLabelShown = true;
                }
            }
            else
            {
                if (ConnectorEndLabel == null)
                {
                    var labelModel = new ConnectorLabelModel(this, false, ShowLableHaderConnection, p);
                    ConnectorEndLabel = labelModel;
                    _isEndpointLabelShown = true;
                }
            }
        }

        protected virtual bool ShowLableHaderConnection => true;

        public IWaypoint Current
        {
            get
            {
                if( _waypointIndex < 0)
                {
                    return new ToWaypointAdapter(this, From);
                }
                else if( _waypointIndex >= _waypoints.Count)
                {
                    return new ToWaypointAdapter(this, To);
                }
                var p = _waypoints.ElementAt(_waypointIndex);
                return p;
            }
        }

        object IEnumerator.Current => Current;

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

        Point GetLabelPosition(bool isStartpointLabel)
        {
            var labelArea = GetLabelArea(isStartpointLabel);
            if (labelArea.Width == 0)
            {
                return GetPositionFromDocking(isStartpointLabel);
            }
            return labelArea.Location;
        }

        public virtual Rect GetLabelArea(bool isStartPointLabel)
        {
            if (isStartPointLabel) return LabelArea;
            return _endPointLabelArea;
        }

        public void SetLabelArea(bool isStartPointLabel, Rect r)
        {
            if (isStartPointLabel)
            {
                LabelArea = r;
            }
            else
            {
                _endPointLabelArea = r;
                RaisePropertyChanged("LabelArea");
            }
        }

        public void SetLabel(bool isStartPointLabel, string l)
        {
            if(isStartPointLabel )
            {
                Label = l;
            }
            else
            {
                _endPointLabel = l;
                RaisePropertyChanged("Label");
            }
        }


        protected virtual Point GetPositionFromDocking( bool isStartPointLabel)
        {
            if( isStartPointLabel)
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
            else
            {
                switch( EndPointDocking)
                {
                    case ConnectorDocking.Top:
                        {
                            double y = ConnectorEnd.Y - LabelDistanceY - LabelDefaultHeight;
                            double x = ConnectorEnd.X + LabelDistanceX;
                            if (ConnectorEnd.X > ConnectorEnd.X)
                            {
                                x = ConnectorEnd.X - LabelDistanceX - LabelDefaultWidht;
                            }
                            return new Point(x, y);
                        }
                    case ConnectorDocking.Bottom:
                        {
                            double y = ConnectorEnd.Y + LabelDistanceY;
                            double x = ConnectorEnd.X + LabelDistanceX;
                            if (ConnectorEnd.X > ConnectorStart.X)
                            {
                                x = ConnectorEnd.X - LabelDistanceX - LabelDefaultWidht;
                            }
                            return new Point(x, y);
                        }
                    case ConnectorDocking.Left:
                        {
                            double y = ConnectorEnd.Y - LabelDistanceY - LabelDefaultHeight;
                            double x = ConnectorEnd.X - LabelDistanceX;
                            if (ConnectorEnd.Y < ConnectorEnd.Y)
                            {
                                y = ConnectorEnd.Y + LabelDistanceY;
                            }
                            return new Point(x, y);
                        }
                    case ConnectorDocking.Right:
                        {
                            double y = ConnectorEnd.Y - LabelDistanceY - LabelDefaultHeight;
                            double x = ConnectorEnd.X - LabelDistanceX - LabelDefaultWidht;

                            if (ConnectorEnd.Y < ConnectorEnd.Y)
                            {
                                y = ConnectorEnd.Y + LabelDistanceY;
                            }
                            return new Point(x, y);
                        }

                    default:
                        throw new NotSupportedException("the end point has no docking");
                        
                }
            }
        }

        Rect GetHotSpotRectangle(Point p)
        {
            var rect = new Rect(new Point(p.X - HotSpotSize / 2, p.Y - HotSpotSize / 2),
                    new Size(HotSpotSize, HotSpotSize));
            return rect;
        }

        void ExecuteShowLabel()
        {
            ShowConnectorLabel(GetLabelPosition(ConnectorStartSelected), ConnectorStartSelected);
        }

        protected virtual bool CanExecuteShowLabel()
        {
            var ret = (_connectorStartLabel == null && _connectorStartSelected) ||
                (_connectorEndLabel == null && _connectorEndSelected);

            return ret;
        }

        void ExecuteHideLabel()
        {
            ConnectorStartLabel = null;
            ConnectorEndLabel = null;
            _isLabelShown = false;
            _isEndpointLabelShown = false;
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

        internal void DeleteWaypoint(int index)
        {
            var wp = Waypoints[index];
            wp.ShapeChanged -= Waypoint_ShapeChanged;
            Waypoints.RemoveAt(index);
            UpdateGeometry();
        }

        internal void AddWaypoint(IWaypoint connectable)
        {
            int i = 0;
            var l = new LineGeometry();
            var p = connectable.Bounds.TopLeft;
            

            try
            {
                List<PathFigure> path = new List<PathFigure>(ConnectorStrategy.ConnectorPath);
                
                foreach (var pf in path) 
                {
                    var start = pf.StartPoint;
                    foreach (var lineSegment in pf.Segments.OfType<LineSegment>())
                    {
                        l.StartPoint = start;
                        l.EndPoint = lineSegment.Point;
                        if (l.FillContains(p, 8, ToleranceType.Absolute)) 
                        {
                            Waypoints.Insert(i, connectable);
                            connectable.GetPreferredConnectorEnd(start, out double relativePos, out ConnectorDocking docking);
                            connectable.GetPreferredConnectorStart(l.EndPoint, out relativePos, out docking);
                            connectable.MiddlePosition = 0.5;
                            connectable.ShapeChanged += Waypoint_ShapeChanged;
                            return;
                        }
                        start = lineSegment.Point;
                    }
                    i++;
                }
            }
            finally
            {
                UpdateGeometry();
            }
        }

        private void Waypoint_ShapeChanged(object sender, OutlineChangedEventArgs e)
        {
            UpdateGeometry();
        }

        public void Dispose(){}

        public bool MoveNext()
        {
            _waypointIndex++;
            return _waypointIndex < _waypoints.Count;
        }

        public void Reset()
        {
            _waypointIndex = -1;
        }


    }
}
