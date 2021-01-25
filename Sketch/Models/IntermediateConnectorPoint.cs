
using Sketch.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sketch.Models
{
    [Serializable]
    class IntermediateConnectorPoint : INotifyPropertyChanged, ISerializable
    {

        public IntermediateConnectorPoint( IConnectable connectable )
        {

        }

        public IntermediateConnectorPoint(SerializationInfo info, StreamingContext context)
        { }

        public event PropertyChangedEventHandler PropertyChanged;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
