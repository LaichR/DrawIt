﻿using System;
using System.Windows;
using System.Windows.Media;
using Sketch.Types;
using Sketch.Interface;
using Sketch.Models;
using Sketch.Models.Geometries;
using System.Runtime.Serialization;
using System.Drawing;

namespace DrawIt.Uml
{
    [Serializable]
    
    [AllowableConnectorTarget(typeof(UmlClassModel))]
    [AllowableConnectorTarget(typeof(UmlPackageModel))]
    public class UmlAssociationModel: ConnectorModel
    {

        public UmlAssociationModel( ConnectionType type, IBoundedItemModel from, IBoundedItemModel to)
            :base(type, from, to)
        {}



        protected UmlAssociationModel(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected override void ComputeGeometry()
        {
            base.ComputeGeometry();
            var g = Geometry as GeometryGroup;
            g.Children.Add(new Arrow
            {
                Translation = new Vector(ConnectorStrategy.ConnectionEnd.X, ConnectorStrategy.ConnectionEnd.Y),
                Rotation = ConnectorStrategy.EndAngle
            }.Ending);
        }

    }
}
