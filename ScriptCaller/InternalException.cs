using System;

namespace ScriptCaller
{
    public class InternalException : Exception
    {
        public InternalException(string message) : base(message)
        {
        }
    }
}