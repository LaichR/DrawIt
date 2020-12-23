﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Sketch.Interface
{
    /// <summary>
    /// Allows to specify which connectors can be used to connect a specific sketch shape
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AllowableConnectorAttribute: Attribute
    {
        readonly Type _allowableConnectorType;

        public AllowableConnectorAttribute(Type cls)
        {
            RuntimeCheck.Contract.Requires(cls != null, "Connector type must not be null");
            RuntimeCheck.Contract.Requires(cls.GetInterface(nameof(IConnectorItemModel)) != null, "Connector type needs to implement the Interface IConnectorItemModel");
            _allowableConnectorType = cls;
        }

        public Type AllowableConnectorType
        {
            get => _allowableConnectorType;
        }

    }
}
