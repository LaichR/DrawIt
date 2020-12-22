using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Sketch.Interface;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Reflection;

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

    public sealed class StructuredAttributeSurrogate : ISerializationSurrogate
    {
        public static readonly List<Type> PersistentClasses = new List<Type>(); 
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var property = obj.GetType().GetProperty("PersistentFields");
            if( property != null)
            {
                PersistencyHelper.BackupPersistentFields(obj, info, (IEnumerable<FieldInfo>)property.GetValue(obj));
            }
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {

            var property = obj.GetType().GetProperty("PersistentFields");
            var getVersion = obj.GetType().GetMethod("GetVersion");
            if (property != null && getVersion != null)
            {
           
                var version = (int)getVersion.Invoke(obj, null);
                PersistencyHelper.RestorePersistentFields(obj, info, (IEnumerable<FieldInfo>)property.GetValue(obj), version);
                
            }
            return obj;
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
