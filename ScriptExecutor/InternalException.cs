using System;

namespace ScriptExecutor
{
    public class InternalException : Exception
    {
        public InternalException(string message) : base(message)
        {
        }
    }
}