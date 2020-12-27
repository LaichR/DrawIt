using System;
using Sketch.Helper.RuntimeCheck;

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
            Contract.Requires(cls != null, "Connector type must not be null");
            Contract.Requires(cls.GetInterface(nameof(IConnectorItemModel)) != null, "Connector type needs to implement the Interface IConnectorItemModel");
            _allowableConnectorType = cls;
        }

        public Type AllowableConnectorType
        {
            get => _allowableConnectorType;
        }

    }
}
