using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Windows;
using System.Windows.Media;
using System.Runtime.Serialization;
using UI.Utilities.Interfaces;
using Sketch.Interface;
using System.ComponentModel;
using RuntimeCheck;
using System.Reflection;
using System.Windows.Navigation;

namespace Sketch.Models
{
    [Serializable]
    public abstract class ModelBase : Prism.Mvvm.BindableBase, IHierarchicalNode, ISketchItemModel
    {

        //FontFamily _labelFont;

        ModelVersion _version = ModelVersion.Undefined;

        List<FieldInfo> _persistentFields = null;

        [PersistentField((int)ModelVersion.V_0_1,"Label",true)]
        string _label = "";

        [PersistentField((int)ModelVersion.V_0_1, "IsSelected",true)]
        bool _isSelected;

        bool _isMarked;

        bool _editModeOn = false;

        [PersistentField((int)ModelVersion.V_0_1, "LabelArea")]
        Rect _labelArea;

        IList<ICommandDescriptor> _commands;

        ////public Guid Id
        //{
        //    get;
        //    protected set;
        //}

        //protected ModelBase(Guid id)
        //{
        //    Id = id;
        //}

        public ModelBase() {}

        public virtual bool IsSerializable
        {
            get => true;
        }

        protected ModelBase(SerializationInfo info, StreamingContext context)
        {
            RestoreFieldData(info, context);
            Initialize();
        }

        public abstract void UpdateGeometry();


        public virtual ModelVersion Version
        {
            get {
                if( _version == ModelVersion.Undefined)
                {
                    _version = ComputeVersion();
                }
                return _version;
            }
        }


        public virtual ISketchItemModel RefModel
        {
            get { return this; }
        }

        public abstract void Move(Transform translation);

        public abstract Geometry Geometry
        {
            get;
        }

        public virtual bool Render(DrawingContext drawingContext) { return false; }

        public virtual void RenderAdornments(DrawingContext drawingContext){}

        public virtual IList<ICommandDescriptor> Commands
        {
            get { return _commands; }
            protected set { _commands = value; }
        }

        public virtual string LabelPropertyName
        {
            get { return nameof(Label); }
        }

        [Browsable(true)]
        public virtual string Label
        {
            get => _label;
            set{ 
                SetProperty<string>(ref _label, value);
      
            }
        }

        public virtual bool IsSelected
        {
            get => _isSelected; 
            set { 
                SetProperty<bool>(ref _isSelected, value);
                IsMarked = false;
            }
        }

        public virtual bool IsMarked
        {
            get => _isMarked;
            set
            {
                SetProperty<bool>(ref _isMarked, value);
            }
        }

        public virtual Rect LabelArea
        {
            get => _labelArea;
            set { SetProperty<Rect>(ref _labelArea, value); }
        }

        public bool AllowEdit
        {
            get => _editModeOn; 
            set {  SetProperty<bool>( ref _editModeOn, value);}
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            PrepareFieldBackup();
            var fields = GetAllPersistentFields();
            info.AddValue(nameof(Version), Version);
            foreach (var f in fields) 
            {
                var persistent = f.GetCustomAttributes(true).OfType<PersistentFieldAttribute>();
                
                var persistentSpec = persistent.First();
                info.AddValue(persistentSpec.Name, f.GetValue(this), f.FieldType);
            }
        }

        public virtual IHierarchicalNode Parent
        {
            get { return null; }
        }

        public void EnterEditMode()
        {
            AllowEdit = true;
        }

        protected virtual void RestoreFieldData(SerializationInfo info, StreamingContext context)
        {
            // see if there is a version in the stream
            ModelVersion version = ModelVersion.V_0_1;
            try
            {
                version = (ModelVersion)info.GetValue(nameof(Version), typeof(ModelVersion));    
            }
            catch { }
            
            var fields = GetAllPersistentFields();

            IEnumerable<string> persistentNames = PersistencyHelper.RestorePersistentFields(this, info, fields, (int)version);

            FieldDataRestored();
            foreach( var n in persistentNames)
            {
                RaisePropertyChanged(n);
            }
        }

        protected IEnumerable<System.Reflection.FieldInfo> GetAllPersistentFields()
        {
            if (_persistentFields == null)
            {
                _persistentFields = new List<FieldInfo>(PersistencyHelper.GetAllPersistentFields(this, typeof(ModelBase).BaseType));
                
            }
            return _persistentFields;
        }

        protected ModelVersion ComputeVersion()
        {
            ModelVersion v = ModelVersion.V_0_1;
            foreach( var f in GetAllPersistentFields())
            {
                if( PersistencyHelper.GetPersistentFieldInfo(f, out PersistentFieldAttribute info))
                {
                    if( v < (ModelVersion)info.AvalailableSince)
                    {
                        v = (ModelVersion)info.AvalailableSince;
                    }
                }
            }
            return v;
        }

        public T GetCustomAttribute<T>() where T: Attribute
        {
            var type = this.GetType();
            return (T)type.GetCustomAttribute<T>();
        }



        protected abstract void Initialize();
        protected virtual void FieldDataRestored() { }
        protected virtual void PrepareFieldBackup() { }
    }
}
