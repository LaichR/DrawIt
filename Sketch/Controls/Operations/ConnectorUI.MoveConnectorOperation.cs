using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Sketch.Models;
using Sketch.Interface;
using Sketch.Types;

namespace Sketch.Controls
{
    public partial class ConnectorUI
    {
        internal class MoveConnectorOperation: IEditOperation
        {
            static readonly Dictionary<ConnectorDocking, List<ConnectorDocking>> _probingOrderDict = new Dictionary<ConnectorDocking, List<ConnectorDocking>>
            {
                { ConnectorDocking.Left, new List<ConnectorDocking> { ConnectorDocking.Bottom, ConnectorDocking.Top, ConnectorDocking.Right, ConnectorDocking.Left } },
                { ConnectorDocking.Right, new List<ConnectorDocking> { ConnectorDocking.Bottom, ConnectorDocking.Top, ConnectorDocking.Left, ConnectorDocking.Right } },
                { ConnectorDocking.Bottom, new List<ConnectorDocking> { ConnectorDocking.Left, ConnectorDocking.Right, ConnectorDocking.Top, ConnectorDocking.Bottom } },
                { ConnectorDocking.Top, new List<ConnectorDocking> { ConnectorDocking.Left, ConnectorDocking.Right, ConnectorDocking.Bottom, ConnectorDocking.Top } }
            };

            bool _done = false;
            readonly ConnectorUI _ui;
            readonly ConnectorModel _model;
            readonly bool _isSelfTransition = false;
            readonly RectangleGeometry _fromGeometry; // used for instersect test
            readonly RectangleGeometry _toGeometry;   // used for intersect test
            readonly ConnectorMoveVisualizer _visualizer;
            readonly IConnectorMoveHelper _moveHelper;
            readonly Action<Point> _mouseMove;
            ConnectorDocking _movingPointDocking;
            ConnectorDocking _otherPointDocking;
            readonly ConnectorDocking _initialOtherPointDocking;
            Point _movePointStart;
            Point _otherPointPosition;
            double _newMoveDistance;
            readonly GeometryGroup _gg = new GeometryGroup();
            Point _p0;
            readonly Point _startPoint;
            readonly ConnectorDocking _allowableDockings;
            bool _moveEndingSuccessful = true;
            public MoveConnectorOperation( ConnectorUI ui, Point p )
            {
                _ui = ui;
                _model = ui._model;
                _startPoint = p;
                _p0 = p;
                _moveHelper = _model.StartMove(p);
                _newMoveDistance = _moveHelper.Distance;
                _movePointStart = _moveHelper.StartPoint;
                var from = _moveHelper.StartingFrom;
                var to = _moveHelper.EndingAt; 
                _fromGeometry = new RectangleGeometry(from.Bounds);
                
                _toGeometry = new RectangleGeometry(to.Bounds);

                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    if( TryAdjustConnectorLine() )
                    {
                        //_moveHelper.Commit()
                    }
                    StopOperation(false);
                }
                else
                {
                    var initialGeometry = _moveHelper.GetGeometry(_gg, _moveHelper.LineType,
                        _movePointStart, _moveHelper.EndPoint, _moveHelper.Distance);

                    _visualizer = new ConnectorMoveVisualizer(initialGeometry); //tbd
                                                                                //_visualizer.MouseMove += HandleMouseMove;
                    _isSelfTransition = _model.From == _model.To;
                    if (_moveHelper.MoveType == MoveType.MoveStartPoint && _model.CanMoveStart)
                    {
                        _mouseMove = MoveStartPoint;
                        _allowableDockings = from.AllowableDockings(false);
                        _movingPointDocking = from.GetConnectorDocking(_moveHelper.StartPoint, false);
                        _otherPointDocking = to.GetConnectorDocking(_moveHelper.EndPoint, true);
                        _otherPointPosition = _moveHelper.EndPoint;
                    }
                    else if (_moveHelper.MoveType == MoveType.MoveEndPoint && _model.CanMoveEnd)
                    {
                        _allowableDockings = to.AllowableDockings(true);
                        _mouseMove = MoveEndPoint;
                        _movingPointDocking = to.GetConnectorDocking(_moveHelper.EndPoint, true);
                        _otherPointDocking = from.GetConnectorDocking(_moveHelper.StartPoint, false);
                        _otherPointPosition = _moveHelper.StartPoint;
                        _movePointStart = _moveHelper.EndPoint;
                    }
                    else if (_moveHelper.MoveType == MoveType.MoveMiddlePoint)
                    {
                        _mouseMove = MoveMiddlePoint;
                    }
                    else
                    {
                        StopOperation(false);
                        return;
                    }
                    _initialOtherPointDocking = _otherPointDocking;
                    _ui.TriggerSnapshot();
                    _ui.MouseMove += HandleMouseMove;
                    _ui.MouseUp += HandleMouseUp;
                    _ui.CaptureMouse();
                    _ui._parent.Canvas.Children.Add(_visualizer);
                }
            }

            public void StopOperation(bool commit)
            {
                if (!_done)
                {
                    _done = true;
                    _ui._parent.Canvas.Children.Remove(_visualizer);
                    _ui.MouseMove -= HandleMouseMove;
                    _ui.MouseUp -= HandleMouseUp;
                    _ui.ReleaseMouseCapture();
                    if (commit && (_startPoint-_p0).Length > 2 )
                    {
                        if(!_moveEndingSuccessful)
                        {
                            _movingPointDocking = ConnectorDocking.Undefined;
                            _newMoveDistance = 0.5;
                            _otherPointDocking = ConnectorDocking.Undefined;
                        }
                        _moveHelper.Commit( _movingPointDocking, _otherPointDocking, _movePointStart, _otherPointPosition, _newMoveDistance );
                    }
                    else
                    {
                        _ui.DropSnapshot();
                    }
                    _ui.UpdateGeometry();
                }
            }

            public void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
            {
                throw new NotImplementedException();
            }

            public void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
            {
                throw new NotImplementedException();
            }

            public void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
            {
                bool commit = e.RightButton == System.Windows.Input.MouseButtonState.Released;
                StopOperation(commit);
                _ui.RegisterHandler(null);
                e.Handled = true;
            }

            public void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
            {
                StopOperation(true);
                _ui.RegisterHandler(null);
                e.Handled = true;
            }

            public void OnMouseMove(System.Windows.Input.MouseEventArgs e)
            {
                Point p = e.GetPosition(_ui._parent.Canvas);

                p.X = Math.Round(p.X / SketchPad.GridSize) * SketchPad.GridSize;
                p.Y = Math.Round(p.Y / SketchPad.GridSize) * SketchPad.GridSize;

                var v = Point.Subtract(_p0, p);
                _p0 = p;
                _mouseMove(p);
                e.Handled = true;
            }

            public void OnKeyDown(System.Windows.Input.KeyEventArgs e)
            {
                if( e.Key == System.Windows.Input.Key.Escape)
                {
                    _ui.DropSnapshot();
                    StopOperation(false);
                }
            }

            public void OnKeyUp(System.Windows.Input.KeyEventArgs e)
            {
                throw new NotImplementedException();
            }

            #region adapters for event handling
            void HandleMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
            {
                OnMouseUp(e);
            }

            void HandleMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
            {
                OnMouseMove(e);
            }

            void MoveStartPoint(Point p) 
            {
                
                _moveHelper.ComputeDockingDuringMove(_moveHelper.StartingFrom.Bounds, p, ref _movingPointDocking, ref _movePointStart);
                //var probings = InitProbings(_movingPointDocking, _otherPointDocking);
                var otherPointPosition = _moveHelper.EndPointRelativePosition;
                var otherPointDocking = _otherPointDocking;
                if (_model.ConnectorStrategy.ConnectionType == ConnectionType.AutoRouting)
                {
                    _otherPointPosition = ProbeOtherPosition(_movePointStart, _moveHelper.EndPoint, _movingPointDocking, _moveHelper.EndingAt.Bounds,
                        ref otherPointPosition, ref _otherPointDocking); ;
                }
                else
                {
                    var lt = (LineType)((int)_movingPointDocking << 8 | (int)  _otherPointDocking);
                    var geometry = _moveHelper.GetGeometry(_gg, lt, _movePointStart, _otherPointPosition, _moveHelper.Distance);
                    _visualizer.UpdateGeometry(geometry);
                }
            }

            void MoveMiddlePoint(Point e) 
            {
                var from = _moveHelper.StartingFrom;
                var to = _moveHelper.EndingAt;
                _moveEndingSuccessful = true;
                if( _isSelfTransition)
                {
                    if (_moveHelper.LineType == LineType.TopTop )
                    {
                        _newMoveDistance = (from.Bounds.Top - e.Y) / 150;
                    }
                    else if (_moveHelper.LineType == LineType.BottomBottom)
                    {
                        _newMoveDistance = (e.Y - from.Bounds.Bottom) / 150;
                    }
                    else if (_moveHelper.LineType == LineType.RightRight)
                    {
                        _newMoveDistance = (e.X - from.Bounds.Right) / 150;
                    }
                    else if (_moveHelper.LineType == LineType.LeftLeft)
                    {
                        _newMoveDistance = (from.Bounds.Left - e.X) / 150;
                    }
                }
                else
                {
                    if (_moveHelper.LineType == LineType.BottomBottom)
                    {
                        var lower = Math.Max(from.Bounds.Bottom, to.Bounds.Bottom);
                        _newMoveDistance = (e.Y - lower) / ComputeConnectorLine.NormalDistance;
                    }
                    else if (_moveHelper.LineType == LineType.TopTop)
                    {
                        var upper = Math.Min(from.Bounds.Top, to.Bounds.Top);
                        _newMoveDistance = (upper - e.Y) / ComputeConnectorLine.NormalDistance;
                    }
                    else if (_moveHelper.LineType == LineType.LeftLeft)
                    {
                        var leftMost = Math.Min(from.Bounds.Left, to.Bounds.Left);
                        _newMoveDistance = (leftMost - e.X) / ComputeConnectorLine.NormalDistance;
                    }
                    else if (_moveHelper.LineType == LineType.RightRight)
                    {
                        var rightMost = Math.Max(from.Bounds.Right, to.Bounds.Right);
                        _newMoveDistance = (e.X - rightMost) / ComputeConnectorLine.NormalDistance;
                    }
                    else if (_moveHelper.LineType == LineType.LeftRight)
                    {
                        var distance = from.Bounds.Left - to.Bounds.Right;
                        _newMoveDistance = (e.X - to.Bounds.Right) / distance;
                    }
                    else if (_moveHelper.LineType == LineType.RightLeft)
                    {
                        var distance = to.Bounds.Left - from.Bounds.Right;
                        _newMoveDistance = (e.X - from.Bounds.Right) / distance;
                    }
                    else if (_moveHelper.LineType == LineType.TopBottom)
                    {
                        var distance = to.Bounds.Top - from.Bounds.Bottom;
                        _newMoveDistance = (e.Y - from.Bounds.Bottom) / distance;
                    }
                    else if (_moveHelper.LineType == LineType.BottomTop)
                    {
                        var distance = from.Bounds.Top - to.Bounds.Bottom;
                        _newMoveDistance = (e.Y - to.Bounds.Bottom) / distance;
                    }
                }
                
                
                _newMoveDistance = Math.Max(0.01, _newMoveDistance);
                var geometry = _moveHelper.GetGeometry(_gg, _moveHelper.LineType, _moveHelper.StartPoint, _moveHelper.EndPoint, _newMoveDistance);
                _visualizer.UpdateGeometry(geometry);
            }

            void MoveEndPoint(Point p) 
            {
                var from = _moveHelper.StartingFrom;
                var to = _moveHelper.EndingAt;
                var otherPointPosition = _moveHelper.StartPointRelativePosition;
                _moveHelper.ComputeDockingDuringMove(to.Bounds, p, ref _movingPointDocking, ref _movePointStart);

                if( _model.ConnectorStrategy.ConnectionType == ConnectionType.AutoRouting)
                {
                    _otherPointPosition = ProbeOtherPosition(_movePointStart, _moveHelper.StartPoint, _movingPointDocking, from.Bounds,
                        ref otherPointPosition, ref _otherPointDocking);
                }
                else
                {   
                    var lt = (LineType)((int)_otherPointDocking << 8 | (int)_movingPointDocking);
                    var geometry = _moveHelper.GetGeometry(_gg, lt, _otherPointPosition, _movePointStart, _moveHelper.Distance);
                    _visualizer.UpdateGeometry(geometry);
                }
            }
            #endregion

            IEnumerable<ConnectorDocking> InitProbings(ConnectorDocking movePointDocking, ConnectorDocking otherPointDocking)
            {
                List<ConnectorDocking> probings = new List<ConnectorDocking>();
                if( !_probingOrderDict.TryGetValue(movePointDocking, out List<ConnectorDocking> defaultOrder))
                {
                    throw new NotSupportedException(string.Format("no probing order defined for {0}", movePointDocking));
                }
                probings.AddRange(defaultOrder);
                probings.Remove(otherPointDocking);
                probings.Insert(0, otherPointDocking);
                return probings;
            }

            Point ProbeOtherPosition( Point start, Point end, ConnectorDocking movingDocking, Rect bounds, ref double relativePosition, ref ConnectorDocking docking)
            {
                _moveEndingSuccessful = false;
                var probings = InitProbings(movingDocking, _initialOtherPointDocking);
                bool changeOtherPointDocking = false;
                var tmpDocking = docking;
                Point tmpPos = end;
                foreach (var d in probings) // pobe the different dockings!
                {
                    tmpDocking = d;
                    LineType lt = _moveHelper.LineType;
                    if (_isSelfTransition)
                    {
                        tmpDocking = movingDocking;
                        lt = (Types.LineType)((int)ConnectorDocking.Self << 8 | (int)movingDocking);
                    }
                    else
                    {
                        lt = (LineType)((int)tmpDocking << 8 | (int)movingDocking);
                    }
                    tmpPos = ConnectorUtilities.ComputePoint(bounds, tmpDocking, relativePosition);

                    var geometry = _moveHelper.GetGeometry(_gg, lt, tmpPos, start, _moveHelper.Distance);

                    if ((_fromGeometry.FillContainsWithDetail(geometry) == IntersectionDetail.Empty &&
                         _toGeometry.FillContainsWithDetail(geometry) == IntersectionDetail.Empty))
                    {
                        changeOtherPointDocking = tmpDocking != docking;
                        _visualizer.UpdateGeometry(geometry);
                        _moveEndingSuccessful = true;
                        break;
                    }
                    System.Diagnostics.Trace.WriteLine("probe another setting");
                }
                if (changeOtherPointDocking)
                {
                    docking = tmpDocking;
                    end = tmpPos;
                }
                return end;
            }

            bool TryAdjustConnectorLine()
            {
                switch(_moveHelper.LineType)
                {
                    case LineType.BottomTop:
                    case LineType.TopBottom:
                        if(Math.Abs(_moveHelper.StartPoint.X-_moveHelper.EndPoint.X)< 3 )
                        {

                        }
                        break;
                    case LineType.LeftRight:
                    case LineType.RightLeft:
                        if (Math.Abs(_moveHelper.StartPoint.X - _moveHelper.EndPoint.X) < 3)
                        {

                        }
                        break;
                }
                return false;
            }


        }
    }
}
