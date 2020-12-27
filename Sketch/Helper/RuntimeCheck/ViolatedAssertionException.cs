using System;
using System.Runtime.Serialization;



namespace Sketch.Helper.RuntimeCheck
{
    [Serializable]
    public class ViolatedAssertionException : SystemException
    {
        public ViolatedAssertionException(string message)
            : base(message) { }
        protected ViolatedAssertionException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }

}
