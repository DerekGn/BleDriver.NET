using BgApiDriver;
using System;

namespace BgApiApp
{
    public class BlueGigaConnectionStatus
    {
        private BgApi.ble_msg_connection_status_evt_t _connectionStatusEvent;

        public BlueGigaConnectionStatus(BgApi.ble_msg_connection_status_evt_t connectionStatusEvent)
        {
            _connectionStatusEvent = connectionStatusEvent ?? throw new ArgumentNullException(nameof(connectionStatusEvent));
        }
        /// <summary>
        /// Remote devices Bluetooth address
        /// </summary>
        public bd_addr Address => _connectionStatusEvent.address;
        /// <summary>
        /// Remote address type
        /// </summary>
        public BluetoothAddressType AddressType => (BluetoothAddressType)_connectionStatusEvent.address_type;
        /// <summary>
        /// Bonding handle if the device has been bonded with.
        /// </summary>
        public int Bonding => _connectionStatusEvent.bonding;
        /// <summary>
        /// Connection handle
        /// </summary>
        public int Connection => _connectionStatusEvent.connection;
        /// <summary>
        /// Current connection interval (units of 1.25ms)
        /// </summary>
        public int ConnectionInterval => _connectionStatusEvent.conn_interval;
        /// <summary>
        /// Connection status flags
        /// </summary>
        public ConnectionStatus Status => (ConnectionStatus)_connectionStatusEvent.flags;
        /// <summary>
        /// Slave latency which tells how many connection intervals the slave may skip.
        /// </summary>
        public int Latency => _connectionStatusEvent.latency;
        /// <summary>
        /// Current supervision timeout (units of 10ms)
        /// </summary>
        public int Timeout => _connectionStatusEvent.timeout;
    }
}