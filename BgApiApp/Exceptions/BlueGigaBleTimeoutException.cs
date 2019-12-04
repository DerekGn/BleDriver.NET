using System;

namespace BgApiApp.Exceptions
{

    [Serializable]
    public class BlueGigaBleTimeoutException : Exception
    {
        public BlueGigaBleTimeoutException() { }
        public BlueGigaBleTimeoutException(string message) : base(message) { }
        public BlueGigaBleTimeoutException(string message, Exception inner) : base(message, inner) { }
        protected BlueGigaBleTimeoutException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
