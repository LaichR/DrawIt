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

namespace Sketch.Models
{
    [Serializable]
    public abstract class ModelBase : Prism.Mvvm.BindableBase, IHierarchicalNode, ISketchItemModel
    {
        
        FontFamily _labelFont;

        string _label = "";

        bool _isSelected;

        bool _isMarked;

        bool _editModeOn = false;

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

        public ModelBase() { }

        public virtual bool IsSerializable
        {
            get => true;
        }

        protected ModelBase(SerializationInfo info, StreamingContext context)
        {
            LabelArea = (System.Windows.Rect)info.GetValue("LabelArea", typeof(System.Windows.Rect));
            Name = info.GetString("Label");
            IsSelected = info.GetBoolean("IsSelected");
        }

        public abstract void UpdateGeometry();

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
            get { return "Name"; }
        }

        public virtual string Name
        {
            get{ return _label; }
            set{ 
                SetProperty<string>(ref _label, value);
            }
        }

        public virtual bool IsSelected
        {
            get { return _isSelected; }
            set { 
                SetProperty<bool>(ref _isSelected, value);
                IsMarked = false;
            }
        }

        public virtual bool IsMarked
        {
            get
            {
                return _isMarked;
            }
            set
            {
                SetProperty<bool>(ref _isMarked, value);
            }
        }

        public virtual Rect LabelArea
        {
            get { return _labelArea; }
            set { SetProperty<Rect>(ref _labelArea, value); }
        }

        public bool AllowEdit
        {
            get { return _editModeOn; }
            set {  SetProperty<bool>( ref _editModeOn, value);}
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Label", Name);
            info.AddValue("LabelArea", LabelArea);
            info.AddValue("IsSelected", IsSelected);
        }

        public virtual IHierarchicalNode Parent
        {
            get { return null; }
        }

        public void EnterEditMode()
        {
            AllowEdit = true;
        }
    }
}
