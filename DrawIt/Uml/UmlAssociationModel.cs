﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
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

        public UmlAssociationModel( ConnectionType type, IBoundedItemModel from, IBoundedItemModel to,
            ISketchItemContainer container)
            :base(type, from, to, container)
        {
            UpdateGeometry();
        }



        protected UmlAssociationModel(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override void UpdateGeometry()
        {
            base.UpdateGeometry();
            var g = Geometry as GeometryGroup;
            g.Children.Add(new Arrow
            {
                Translation = new Vector(ConnectorStrategy.ConnectionEnd.X, ConnectorStrategy.ConnectionEnd.Y),
                Rotation = ConnectorStrategy.EndAngle
            }.Ending);
        }

    }
}
