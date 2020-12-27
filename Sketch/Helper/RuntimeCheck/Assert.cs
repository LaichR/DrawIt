using System;
using System.Runtime.Serialization;
    

namespace Sketch.Helper.RuntimeCheck
{
    

    public static class Assert
    {
        public static void True(bool condition, string message, params object[] args )
        {
            if( !condition)
            {
                throw new ViolatedAssertionException(
                    string.Format(message, args));
            }
        }

        public static T IsOfType<T>(object obj) where T:class
        {
            if (obj is T value) return value;

            throw new ViolatedAssertionException(
                    string.Format("An object of type {0} cannot be compared to an object of type <BoundsComparer>", obj.GetType().Name));
            
        }
    }


}
