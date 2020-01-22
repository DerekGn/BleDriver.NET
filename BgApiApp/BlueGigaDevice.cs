using BgApiDriver;
using System;
using System.Collections.Generic;
using static BgApiDriver.BgApi;

namespace BgApiApp
{
    public class BlueGigaDevice : BaseBlueGiga
    {
        private BlueGigaAdvertisement _advertisement;
        private List<BlueGigaService> _services;
        private BlueGigaBleAdapter _adapter;
        
        public BlueGigaDevice(BlueGigaBleAdapter adapter, BlueGigaAdvertisement advertisement)
        {
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            _advertisement = advertisement ?? throw new ArgumentNullException(nameof(advertisement));

            _services = new List<BlueGigaService>();
        }

        public BlueGigaConnectionStatus ConnectionStatus { get; private set; }

        public void Connect()
        {
            var response = ExecuteOperation(() =>
            {
                return _adapter.ble_cmd_gap_connect_direct(
                    _advertisement.ScanResponseEvent.sender,
                    _advertisement.ScanResponseEvent.address_type,
                    60, // 60 * 1.25ms
                    76, // 72 * 1.25ms
                    100, // 100 * 10ms
                    0);
            });

            _adapter.WaitForEvent((evt) =>
            {
                if (evt is ble_msg_connection_status_evt_t connectStatusEvent &&
                    connectStatusEvent.connection == response.connection_handle)
                {
                    ConnectionStatus = new BlueGigaConnectionStatus(connectStatusEvent);
                    return EventProcessingResult.Complete;
                }

                return EventProcessingResult.Skip;
            });
        }

        public void Disconnect()
        {
            var response = ExecuteOperation(() =>
            {
                return _adapter.ble_cmd_connection_disconnect(ConnectionStatus.ConnectionHandle);
            });

            _adapter.WaitForEvent((evt) =>
            {
                if (evt is ble_msg_connection_disconnected_evt_t disconnectEvent &&
                    disconnectEvent.connection == ConnectionStatus.ConnectionHandle)
                {
                    return EventProcessingResult.Complete;
                }

                return EventProcessingResult.Skip;
            });
        }

        public void EncryptConnection()
        {
            var response = ExecuteOperation(() =>
            {
                return _adapter.ble_cmd_sm_encrypt_start(ConnectionStatus.ConnectionHandle, ConnectionStatus.Bonding);
            });

            _adapter.WaitForEvent((evt) =>
            {
                if (evt is ble_msg_sm_bonding_fail_evt_t bondingFailedEvent &&
                    bondingFailedEvent.handle == ConnectionStatus.ConnectionHandle)
                {
                    return EventProcessingResult.Complete;
                }

                if (evt is ble_msg_connection_status_evt_t connectionStatsEvent &&
                    connectionStatsEvent.connection == ConnectionStatus.ConnectionHandle)
                {
#warning TODO update the connectionStatus
                    return EventProcessingResult.Complete;
                }

                return EventProcessingResult.Skip;
            });

#warning TODO CHECK result
        }

        public IReadOnlyCollection<BlueGigaService> GetGattServices()
        {
            var response = ExecuteOperation(() =>
            {
                return _adapter.ble_cmd_attclient_read_by_group_type(ConnectionStatus.ConnectionHandle, 0x1, int.MaxValue, new byte[] { 0, 0x28 });
            });

            _services.Clear();

            ble_msg_attclient_procedure_completed_evt_t e = null;

            _adapter.WaitForEvent((evt) =>
            {
                if (evt is ble_msg_attclient_group_found_evt_t attClientGroupFoundEvent)
                {
                    _services.Add(new BlueGigaService(_adapter, attClientGroupFoundEvent));
                    return EventProcessingResult.Processed;
                }
                else if (evt is ble_msg_attclient_procedure_completed_evt_t attClientProcedureCompleteEvent)
                {
                    e = attClientProcedureCompleteEvent;

                    return EventProcessingResult.Complete;
                }
                else
                {
                    return EventProcessingResult.Skip;
                }
            });

#warning TODO CHECK result

            if(e != null && e.result != (int)ble_error.ble_err_success)
            {
                throw new Exception();
            }

            return _services;
        }
    }
}