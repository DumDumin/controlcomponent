using System;

namespace ControlComponents.Core
{
    public class OrderOutputException : Exception
    {
        public OrderOutputException(string message) : base(message)
        {
        }
    }
}