using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sketch.Models;
using System.Windows;
using System.Windows.Media;
using UI.Utilities.Interfaces;
using Sketch.Interface;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace DrawIt.Uml
{
    [Serializable]
    [AllowableConnector(typeof(UmlDependencyModel))]
    [AllowableConnector(typeof(UmlAssociationModel))]
    [AllowableConnector(typeof(UmlCompositionModel))]
    [AllowableConnector(typeof(UmlGeneralizationModel))]
    
    public class UmlClassModel: ContainerModel
    {
        new const int DefaultWidth = 150;
        new const int DefaultHeight = 70;

        [PersistentField((int)ModelVersion.V_2_2, "Members", true)]
        readonly ObservableCollection<UmlMemberDescription> _members = new ObservableCollection<UmlMemberDescription>();

        static UmlClassModel()
        {
            
            Sketch.PropertyEditor.PropertyEditTemplateSelector.RegisterDataTemplate(typeof(ObservableCollection<UmlMemberDescription>), "ClassMembersTemplate");
            Sketch.PropertyEditor.PropertyDisplayTemplateSelector.RegisterDataTemplate(typeof(ObservableCollection<UmlMemberDescription>), "ClassMembersTemplate");
        }

        public UmlClassModel(Point p)
            : base(p, new Size(DefaultWidth, DefaultHeight)) 
        {
            CanChangeSize = true;
            CanEditLabel = true;
            Label = "new class";
            RotationAngle = 0.0;
            _members.CollectionChanged += Members_CollectionChanged; ;
            //UpdateGeometry();
        }



        private void Members_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach( var meberDescription in e.NewItems.OfType<UmlMemberDescription>() )
                {
                    meberDescription.PropertyChanged += MeberDescription_PropertyChanged;
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var meberDescription in e.OldItems.OfType<UmlMemberDescription>())
                {
                    meberDescription.PropertyChanged -= MeberDescription_PropertyChanged;
                }
            }

            RaisePropertyChanged(nameof(MemberList));
        }

        private void MeberDescription_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if( e.PropertyName == nameof(UmlMemberDescription.IsPublic))
            {
                RaisePropertyChanged(nameof(MemberList));
            }
        }

        [Browsable(true)]
        public ObservableCollection<UmlMemberDescription> Members
        {
            get => _members;
        }

        public IList<UmlMemberDescription> MemberList
        {
            get => new List<UmlMemberDescription>(_members.Where((x)=>x.IsPublic));
        }

        protected override Rect ComputeLabelArea(string label)
        {
            var location = new Point(5, 5);
            return new Rect(location, new Size(Bounds.Width, 20)); ;
        }


        protected UmlClassModel(SerializationInfo info, StreamingContext context) : base(info, context) {}

        public override void UpdateGeometry()
        {
            var myGeometry = Geometry as GeometryGroup;
            myGeometry.Children.Clear();
            myGeometry.Children.Add(new RectangleGeometry(
                new Rect(0, 0, Bounds.Width, Bounds.Height), 2, 2));
            myGeometry.Transform = Rotation;
        }

        protected override void FieldDataRestored()
        {
            base.FieldDataRestored();
            foreach( var m in Members)
            {
                m.PropertyChanged += MeberDescription_PropertyChanged;
            }
            Members.CollectionChanged += Members_CollectionChanged;

        }


    }
}
