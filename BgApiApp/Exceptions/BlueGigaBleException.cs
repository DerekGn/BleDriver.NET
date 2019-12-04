using System;

namespace BgApiApp.Exceptions
{
    [Serializable]
    public class BlueGigaBleException : Exception
    {
        public BlueGigaBleException(int result)
        {
            Result = result;
        }
        public BlueGigaBleException(string message, int result) : base(message)
        {
            Result = result;
        }
        public BlueGigaBleException(string message, System.Exception inner, int result) : base(message, inner)
        {
            Result = result;
        }
        protected BlueGigaBleException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public int Result { get; }
    }
}
