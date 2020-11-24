using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Sketch.Interface;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Sketch.Models
{
    
    sealed class SketchItemContainerProxy
    {
        ISketchItemContainer _realContainer;
        
        public SketchItemContainerProxy() { }

        public SketchItemContainerProxy(ISketchItemContainer realContainer )
        {
            _realContainer = realContainer;
        }

        internal ISketchItemContainer Container
        {
            get => _realContainer;
            set => _realContainer = value;
        }
    }

    sealed class SketchItemContainerSerializationSurrogate : ISerializationSurrogate
    {
        readonly ISketchItemContainer _container;

        internal SketchItemContainerSerializationSurrogate(ISketchItemContainer container)
        {
            _container = container;
        }

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {}

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var proxy = (SketchItemContainerProxy)obj;
            proxy.Container = _container;
            return proxy;
        }
    }
}
