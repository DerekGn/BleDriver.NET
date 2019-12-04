using System;

namespace BgApiApp
{
    [Flags]
    public enum ConnectionStatus
    {
        /// <summary>
        /// This status flag tells the connection exists to a remote device.
        /// </summary>
        Connected = 0,
        /// <summary>
        /// This flag tells the connection is encrypted.
        /// </summary>
        Encrypted = 2,
        /// <summary>
        /// Connection completed flag, which is used to tell a new connection has been created.
        /// </summary>
        Completed = 4,
        /// <summary>
        /// This flag tells that connection parameters have changed and. It is set when connection parameters have changed due to a link layer operation.
        /// </summary>
        ParametersChange = 8
    }
}
