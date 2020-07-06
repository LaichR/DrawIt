using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Sketch.Models;
using Sketch.Types;

namespace Sketch.Controls
{
    public partial class ConnectorUI
    {
        internal class MoveConnectorOperation: IEditOperation
        {
            bool _done = false;
            ConnectorUI _ui;
            ConnectorModel _model;
            bool _isSelfTransition = false;
            RectangleGeometry _fromGeometry; // used for instersect test
            RectangleGeometry _toGeometry;   // used for intersect test
            ConnectorMoveVisualizer _visualizer;
            IConnectorMoveHelper _moveHelper;
            Action<Point> _mouseMove;
            ConnectorDocking _movingPointDocking;
            ConnectorDocking _otherPointDocking;
            Point _movePointStart;
            Point _otherPointPosition;
            double _newMoveDistance;
            GeometryGroup _gg = new GeometryGroup();
            Point _p0;
            List<double> _moveSpeed = new List<double>();
            
            List<ConnectorDocking> _probings = new List<ConnectorDocking> 
                { ConnectorDocking.Bottom, ConnectorDocking.Left, ConnectorDocking.Right, ConnectorDocking.Top};

            public MoveConnectorOperation( ConnectorUI ui, Point p )
            {
                _ui = ui;
                _model = ui._model;
   
                _p0 = p;
                _moveHelper = _model.StartMove(p);
                _newMoveDistance = _moveHelper.Distance;
                _movePointStart = _moveHelper.StartPoint;
                var fromRect = _model.From.Bounds; //fromRect.Inflate(-4, -4); fromRect.Offset(2, 2);
                var toRect = _model.To.Bounds; //toRect.Inflate(-4, -4); toRect.Offset(2, 2);
                _fromGeometry = new RectangleGeometry(fromRect);
                
                _toGeometry = new RectangleGeometry(toRect);
                
                var initialGeometry = _moveHelper.GetGeometry(_gg, _moveHelper.LineType,
                    _model.ConnectorStart, _model.ConnectorEnd, _moveHelper.Distance);

                _visualizer = new ConnectorMoveVisualizer(initialGeometry); //tbd
                _visualizer.MouseMove += HandleMouseMove;
                _isSelfTransition = ((int)_moveHelper.LineType >> 8) == (int)Types.ConnectorDocking.Self;
                if( _moveHelper.MoveType == MoveType.MoveStartPoint)
                {
                    _mouseMove = MoveStartPoint;
                    _movingPointDocking = _model.StartPointDocking;
                    _otherPointDocking = _model.EndPointDocking;
                    _otherPointPosition = _model.ConnectorEnd;
                }
                else if( _moveHelper.MoveType == MoveType.MoveEndPoint)
                {
                    _mouseMove = MoveEndPoint;
                    _movingPointDocking = _model.EndPointDocking;
                    _otherPointDocking = _model.StartPointDocking;
                    _otherPointPosition = _model.ConnectorStart;
                }
                else if( _moveHelper.MoveType == MoveType.MoveMiddlePoint)
                {
                    _mouseMove = MoveMiddlePoint;
                    _movingPointDocking = _model.StartPointDocking;
                    _otherPointDocking = _model.EndPointDocking;
                    _movePointStart = _model.ConnectorStart;
                    _otherPointPosition = _model.ConnectorEnd;
                }
                else
                {
                    StopOperation(false);
                    return;
                }
                _ui.TriggerSnapshot();
                _probings.Remove(_otherPointDocking);
                _probings.Insert(0, _otherPointDocking);
                _ui.MouseMove += HandleMouseMove;
                _ui.MouseUp += HandleMouseUp;
                _ui.CaptureMouse();
                _ui._parent.Canvas.Children.Add(_visualizer);
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
                    if (commit)
                    {
                        _moveHelper.Commit(_movingPointDocking, _otherPointDocking, _movePointStart, _otherPointPosition, _newMoveDistance);
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
                //var currentSpeed = v.Length;
                //_moveSpeed.Add(currentSpeed);
                //if( _moveSpeed.Count > 32)
                //{
                //    _moveSpeed.RemoveAt(0);
                //}
                //System.Diagnostics.Trace.WriteLine(string.Format("current speed = {0}", currentSpeed));
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
                _moveHelper.ComputeDockingDuringMove(_model.From.Bounds, p, ref _movingPointDocking, ref _movePointStart);
                bool changeOtherPointDocking = false;
                var tmpDocking = ConnectorDocking.Undefined;
                var tmpPos = _otherPointPosition;
                foreach( var d in _probings)
                {
                    
                    LineType lt = _moveHelper.LineType;
                    if (_isSelfTransition)
                    {
                        tmpDocking = _movingPointDocking;
                        lt = (Types.LineType)((int)Types.ConnectorDocking.Self << 8| (int)_movingPointDocking);
                    }
                    else
                    {
                        tmpDocking = d;
                        lt = (LineType)((int)_movingPointDocking << 8 | (int)tmpDocking);
                    }
                    tmpPos = ConnectorUtilities.ComputePoint(_model.To.Bounds, tmpDocking, _model.EndPointRelativePosition);
                    var geometry = _moveHelper.GetGeometry(_gg, lt, _movePointStart, tmpPos, _moveHelper.Distance);
                    //if( geometry is PathGeometry)
                    //{
                    //    var path = geometry as PathGeometry;
                    //    var pathFigure = path.Figures.First();
                    //    pathFigure.StartPoint
                    //}
                    if ((_fromGeometry.FillContainsWithDetail(geometry) == IntersectionDetail.Empty &&
                          _toGeometry.FillContainsWithDetail(geometry) == IntersectionDetail.Empty))
                    {
                        
                        changeOtherPointDocking = tmpDocking != _otherPointDocking;
                        _visualizer.UpdateGeometry(geometry);
                        
                        break;
                    }
                    System.Diagnostics.Trace.WriteLine("intersection with geometry");
                }
                if (changeOtherPointDocking)
                {
                    _otherPointDocking = tmpDocking;
                    _otherPointPosition = tmpPos;
                    //_probings.Remove(tmpDocking);
                    //_probings.Insert(0, tmpDocking);
                }
               
   
            }

            void MoveMiddlePoint(Point e) 
            {
                if( _moveHelper.LineType == LineType.BottomBottom)
                {
                    var lower = Math.Max( _model.From.Bounds.Bottom, _model.To.Bounds.Bottom);
                    _newMoveDistance = (e.Y - lower) / ComputeConnectorLine.NormalDistance;
                }
                else if(_moveHelper.LineType == LineType.TopTop)
                {
                    var upper = Math.Min(_model.From.Bounds.Top, _model.To.Bounds.Top);
                    _newMoveDistance = (upper - e.Y) / ComputeConnectorLine.NormalDistance;
                }
                else if( _moveHelper.LineType == LineType.LeftLeft)
                {
                    var leftMost = Math.Min(_model.From.Bounds.Left, _model.To.Bounds.Left);
                    _newMoveDistance = (leftMost - e.X) / ComputeConnectorLine.NormalDistance;
                }
                else if( _moveHelper.LineType == LineType.RightRight)
                {
                    var rightMost = Math.Max(_model.From.Bounds.Right, _model.To.Bounds.Right);
                    _newMoveDistance = (e.X - rightMost) / ComputeConnectorLine.NormalDistance;
                }
                else if (_moveHelper.LineType == LineType.LeftRight )
                {
                    var distance = _model.From.Bounds.Left - _model.To.Bounds.Right;
                    _newMoveDistance = (e.X - _model.To.Bounds.Right) / distance;
                }
                else if( _moveHelper.LineType == LineType.RightLeft)
                {
                    var distance = _model.To.Bounds.Left - _model.From.Bounds.Right;
                    _newMoveDistance = (e.X - _model.From.Bounds.Right) / distance;
                }
                else if (_moveHelper.LineType == LineType.TopBottom )
                {
                    var distance = _model.To.Bounds.Top - _model.From.Bounds.Bottom;
                    _newMoveDistance = (e.Y - _model.From.Bounds.Bottom ) / distance;
                }
                else if (_moveHelper.LineType == LineType.BottomTop)
                {
                    var distance = _model.From.Bounds.Top - _model.To.Bounds.Bottom;
                    _newMoveDistance = (e.Y - _model.To.Bounds.Bottom) / distance;
                }
                else if( _moveHelper.LineType == LineType.SelfTop)
                {
                    _newMoveDistance = (_model.From.Bounds.Top - e.Y) / 150;
                }
                else if (_moveHelper.LineType == LineType.SelfBottom)
                {
                    _newMoveDistance = (e.Y - _model.From.Bounds.Bottom) / 150;
                }
                else if (_moveHelper.LineType == LineType.SelfRight)
                {
                    _newMoveDistance = (e.X - _model.From.Bounds.Right) / 150;
                }
                else if (_moveHelper.LineType == LineType.SelfLeft)
                {
                    _newMoveDistance = (_model.From.Bounds.Left - e.X ) / 150;
                }
                _newMoveDistance = Math.Max(0.01, _newMoveDistance);
                var geometry = _moveHelper.GetGeometry(_gg, _moveHelper.LineType, _model.ConnectorStart, _model.ConnectorEnd, _newMoveDistance);
                _visualizer.UpdateGeometry(geometry);
            }

            void MoveEndPoint(Point p) 
            {

                _moveHelper.ComputeDockingDuringMove(_model.To.Bounds, p, ref _movingPointDocking, ref _movePointStart);


                bool changeOtherPointDocking = false;
                var tmpDocking = ConnectorDocking.Undefined;
                var tmpPos = _otherPointPosition;
                foreach (var d in _probings)
                {
                    tmpDocking = d;
                    LineType lt = _moveHelper.LineType;
                    if (_isSelfTransition)
                    {
                        tmpDocking = _movingPointDocking;
                        lt = (Types.LineType)((int)Types.ConnectorDocking.Self << 8 | (int)_movingPointDocking);
                    }
                    else
                    {
                        lt = (LineType)((int)tmpDocking << 8 | (int)_movingPointDocking);
                    }
                    tmpPos = ConnectorUtilities.ComputePoint(_model.From.Bounds, tmpDocking, _model.StartPointRelativePosition);
                    
                    var geometry = _moveHelper.GetGeometry(_gg, lt, tmpPos, _movePointStart, _moveHelper.Distance);
                    if ((_fromGeometry.FillContainsWithDetail(geometry) == IntersectionDetail.Empty &&
                          _toGeometry.FillContainsWithDetail(geometry) == IntersectionDetail.Empty))
                    {
                        changeOtherPointDocking = tmpDocking != _otherPointDocking;
                        _visualizer.UpdateGeometry(geometry);
                        break;
                    }
                }
                if (changeOtherPointDocking)
                {
                    _otherPointDocking = tmpDocking;
                    _otherPointPosition = tmpPos;
                    //_probings.Remove(tmpDocking);
                    //_probings.Insert(0, tmpDocking);
                }
            }
            #endregion

            

            

            
        }
    }
}
