﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Sketch.Types;
using Sketch.Interface;

namespace Sketch.Models
{
    public interface IConnectorMoveHelper
    {
        Geometry GetGeometry(LineType lineType, Point start, Point end, double distance);

        void Commit(ConnectorDocking movePointDocking, ConnectorDocking otherPointDocking, Point newPositionStartPoint, Point newPositionEndPoint, double newDistance);

        void ComputeDockingDuringMove(Rect rect, Point p, ref ConnectorDocking currentDocking, ref Point lastPos);

        Point StartPoint
        {
            get;
        }

        ConnectionType ConnectionType
        {
            get;
        }

        LineType LineType
        {
            get;
        }

        double Distance
        {
            get;
        }

        MoveType MoveType
        {
            get;
        }
    }
}
