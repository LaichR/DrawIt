using System;


namespace Sketch.Helper.RuntimeCheck
{
    /// <summary>
    /// The class contract is motivated by the class Contract in the namespace System.Diagnostics.Contract. Unfortunately CodeContracts were not supported in
    /// Visual Studio since version 2015. Nevertheless I wanted to keep at least a consistent standard way to do the parameter testing.
    /// So: What is left is not anymore comparable with what the original CodeContracts used to be. 
    /// It just offers a way of checking parameters for functions and giving out an appropriate error message in case of a 'contract violoation'
    /// </summary>
    public static class Contract
    {
        /// <summary>
        /// Used for function argument checking.
        /// </summary>
        /// <typeparam name="T">Exception that is thrown if the condition is not met</typeparam>
        /// <param name="condition">Condition that is checked for one or several function arguments</param>
        /// <param name="message">The error message that is associated with the contract violation</param>
        /// <param name="args">Possibly additional arguments to be included in the error</param>
        public static void Requires<T>(bool condition, string message, params object[] args) where T : Exception
        {
            if (!condition)
            {
                var ctor = typeof(T).GetConstructor(new[] { typeof(string) });
                var exception = (Exception)ctor.Invoke(new object[] { string.Format(message, args) });
                throw exception;
            }
        }

        /// <summary>
        /// Used for function argument checking.
        /// Without specifying a exception type, the thrown exception will be of type ViolatedContractException
        /// </summary>
        /// <param name="condition">Condition that is checked for one or several function arguments</param>
        /// <param name="message">The error message that is associated with the contract violation</param>
        /// <param name="args">Possibly additional arguments to be included in the error</param>
        public static void Requires(bool condition, string message, params object[] args)
        {
            Requires<ViolatedContractException>(condition, message, args);
        }
    }
}
