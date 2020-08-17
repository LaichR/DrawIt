﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Sketch.Types;
using Sketch.Models;
using Sketch.Interface;
using Sketch.Models.Geometries;
using System.Runtime.Serialization;
using System.Collections;

namespace DrawIt.Uml
{
    [Serializable]

    [AllowableConnectorTarget(typeof(UmlFinalStateModel))]
    [AllowableConnectorTarget(typeof(UmlChoiceModel))]
    [AllowableConnectorTarget(typeof(UmlActivityModel))]
    [AllowableConnectorTarget(typeof(UmlActionConnector))]
    public class UmlActivityDiagramEdge : ConnectorModel
    {
        public UmlActivityDiagramEdge( ConnectionType type, IBoundedItemModel from, IBoundedItemModel to,
            ISketchItemContainer container)
            :base(type, from, to, container)
        {}



        protected UmlActivityDiagramEdge(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }

        public override void UpdateGeometry()
        {
            base.UpdateGeometry();
            var g = Geometry as GeometryGroup;
            if (g != null)
            {
                g.Children.Add(new Arrow
                {
                    Translation = new Vector(ConnectorStrategy.ConnectionEnd.X, ConnectorStrategy.ConnectionEnd.Y),
                    Rotation = ConnectorStrategy.EndAngle
                }.Ending);
            }
        }

    }
}