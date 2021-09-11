using System;
using System.Runtime.Serialization;

namespace ControlComponents.Core
{
    [Serializable]
    public class ExecutionException : Exception
    {
        public ExecutionException(string message) : base(message)
        {
        }
    }
}