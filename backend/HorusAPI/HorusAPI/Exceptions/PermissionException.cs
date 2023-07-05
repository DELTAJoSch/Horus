using System.Runtime.Serialization;

namespace HorusAPI.Exceptions
{
    /// <summary>
    /// Exception that signifies that an action does not fit the correct role
    /// </summary>
    public class PermissionException : Exception
    {
        public PermissionException()
        {
        }

        public PermissionException(string? message) : base(message)
        {
        }

        public PermissionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PermissionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
