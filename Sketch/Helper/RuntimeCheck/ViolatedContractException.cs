using System;
using System.Runtime.Serialization;

namespace Sketch.Helper.RuntimeCheck
{

    class ViolatedContractException: Exception
    {
        public ViolatedContractException(string message)
            : base(message) { }
        protected ViolatedContractException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
