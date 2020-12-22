using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sketch.Models;
using System.Runtime.Serialization;


namespace DrawIt.Uml
{
    [Serializable]
    public class UmlMemberDescription: StructuredAttribute<UmlMemberDescription>
    {

        [PersistentField(1, "Name")]
        string _name;
        [PersistentField(1, "DataType")]
        string _datatype;
        [PersistentField(1, "IsPublic", true)]
        bool _ispublic;

        public UmlMemberDescription():base() { }

        UmlMemberDescription(SerializationInfo info, StreamingContext context):base(info, context) { }

        public string Name
        {
            get => _name;
            set => SetProperty<string>(ref _name, value);
        }

        public string DataType
        {
            get => _datatype;
            set => SetProperty<string>(ref _datatype, value);
        }

        public bool IsPublic
        {
            get => _ispublic;
            set => SetProperty<bool>(ref _ispublic, value);
        }
    }
}
