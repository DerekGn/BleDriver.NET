using BgApiDriver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using static BgApiDriver.BgApi;

namespace BgApiApp
{
    public class BlueGigaCharacteristic : BaseBlueGiga
    {
        private ble_msg_attclient_attribute_value_evt_t _attClientAttributeValueEvent;
        private BlueGigaBleAdapter _adapter;

        public BlueGigaCharacteristic(BlueGigaBleAdapter adapter, ble_msg_attclient_attribute_value_evt_t attClientAttributeValueEvent)
        {
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            _attClientAttributeValueEvent = attClientAttributeValueEvent ?? throw new ArgumentNullException(nameof(attClientAttributeValueEvent));

            IsBroadcastSupported = ((_attClientAttributeValueEvent.value[0] & 1) == 1);
            IsReadSupported = ((_attClientAttributeValueEvent.value[0] & 2) == 2);
            IsWriteWithoutAcknowledgmentSupported = ((_attClientAttributeValueEvent.value[0] & 4) == 4);
            IsWriteSupported = ((_attClientAttributeValueEvent.value[0] & 8) == 8);
            IsNotificationSupported = ((_attClientAttributeValueEvent.value[0] & 16) == 16);
            IsIndicationSupported = ((_attClientAttributeValueEvent.value[0] & 32) == 32);
            IsAuthenticatedWrite = ((_attClientAttributeValueEvent.value[0] & 64) == 64);
            IsAdditionalPropertiesAvailable = ((_attClientAttributeValueEvent.value[0] & 128) == 128);

            Properties = _attClientAttributeValueEvent.value[0];

            ValueAttributeHandle = BitConverter.ToUInt16(_attClientAttributeValueEvent.value, 1);

            Uuid = BitConverter.ToString(_attClientAttributeValueEvent.value.Reverse().ToArray(), 0, (_attClientAttributeValueEvent.value.Length - 3)).Replace("-", "");
        }

        public int Handle => _attClientAttributeValueEvent.atthandle;
        public bool IsBroadcastSupported { get; }
        public bool IsReadSupported { get; }
        public bool IsWriteWithoutAcknowledgmentSupported { get; }
        public bool IsWriteSupported { get; }
        public bool IsNotificationSupported { get; }
        public bool IsIndicationSupported { get; }
        public bool IsAuthenticatedWrite { get; }
        public bool IsAdditionalPropertiesAvailable { get; }
        public byte Properties { get; }
        public ushort ValueAttributeHandle { get; }
        public string Uuid { get; }

        public ReadOnlyCollection<byte> Read()
        {
            var response = ExecuteOperation(() =>
            {
                return _adapter.ble_cmd_attclient_read_by_handle(
                    _attClientAttributeValueEvent.connection,
                    _attClientAttributeValueEvent.atthandle);
            });

            List<byte> result = null;

            _adapter.WaitForEvent((evt) =>
            {
                if (evt is ble_msg_attclient_attribute_value_evt_t attClientAttributeValueEvent)
                {
                    result = evt.Data.ToList();

                    return EventProcessingResult.Complete;
                }
                
                return EventProcessingResult.Processed;
            });

            return result.AsReadOnly();
        }

        public void EnableNotificationAsync()
        {
            var response = ExecuteOperation<BgApiResponse>(() =>
            {
                //if (IsWriteSupported)
                    return _adapter.ble_cmd_attclient_attribute_write(_attClientAttributeValueEvent.connection, Handle, new byte[] { 0x01, 0x00 });
                //else
                //    return _adapter.ble_cmd_attclient_write_command(_attClientAttributeValueEvent.connection, Handle, new byte[] { 0x01, 0x00 });
            });

#warning TODO replace with method
            _adapter.WaitForEvent((evt) =>
            {
                if (evt is ble_msg_attclient_procedure_completed_evt_t attClientProcedureCompleteEvent &&
                        attClientProcedureCompleteEvent.connection == _attClientAttributeValueEvent.connection)
                {
                    return EventProcessingResult.Complete;
                }

                return EventProcessingResult.Skip;
            });
        }

        public void DisableNotificationAsync()
        {
            var response = ExecuteOperation<BgApiResponse>(() =>
            {
                if (IsWriteSupported)
                    return _adapter.ble_cmd_attclient_attribute_write(_attClientAttributeValueEvent.connection, Handle, new byte[] { 0x00, 0x00 });
                else
                    return _adapter.ble_cmd_attclient_write_command(_attClientAttributeValueEvent.connection, Handle, new byte[] { 0x00, 0x00 });
            });

#warning TODO replace with method
            _adapter.WaitForEvent((evt) =>
            {
                if (evt is ble_msg_attclient_procedure_completed_evt_t attClientProcedureCompleteEvent &&
                        attClientProcedureCompleteEvent.connection == _attClientAttributeValueEvent.connection)
                {
                    return EventProcessingResult.Complete;
                }

                return EventProcessingResult.Skip;
            });
        }
    }
}