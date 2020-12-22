using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Sketch.Helper;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Serialization;

namespace Sketch.Models
{
    [Serializable]
    public abstract class StructuredAttribute<T> : INotifyPropertyChanged, ISerializable
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        readonly static List<FieldInfo> _persistentFields = new List<FieldInfo>();
        static int _version = -1;

        protected StructuredAttribute() { }
        protected StructuredAttribute(SerializationInfo info, StreamingContext context) 
        {
            PersistencyHelper.RestorePersistentFields(this, info, PersistentFields, GetVersion());
            FieldDataRestored();
        }

        protected virtual void FieldDataRestored() { }

        public void SetProperty<T1>(ref T1 backup, T1 value, [CallerMemberName] string name = "")
        {
            backup = value;
            RaisePropertyChanged(name);
        }

        public IList<FieldInfo> PersistentFields
        {
            get
            {
                if( _persistentFields.Count == 0)
                {
                    _persistentFields.AddRange(PersistencyHelper.GetAllPersistentFields(this, typeof(StructuredAttribute<T>)));
                }
                return _persistentFields;
            }
        }
        public void RaisePropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public virtual int GetVersion()
        {
            if( _version < 0)
            {
                _version = PersistencyHelper.GetMaxVersion(PersistentFields);
            }
            return _version;
        }

        protected virtual void PrepareFieldBackup() { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            PrepareFieldBackup();
            PersistencyHelper.BackupPersistentFields(this, info, PersistentFields);
        }
    }
}