using System;
using System.Runtime.Serialization;

namespace ControlComponent
{
    [Serializable]
    public class ExecutionException : Exception
    {
        public ExecutionException()
        {
        }

        public ExecutionException(string message) : base(message)
        {
        }

        public ExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}