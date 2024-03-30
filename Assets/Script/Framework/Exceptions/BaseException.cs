using System;
using System.Runtime.CompilerServices;

namespace Corby.Framework.Exceptions
{
    public class BaseException : Exception
    {
        public BaseException(string message, [CallerMemberName] string memberName = null) 
            : base($"[:{memberName}] - {message}")
        {
        }

        public BaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}