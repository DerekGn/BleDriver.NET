using System;
using System.Collections.Generic;
using System.Linq;

using static BgApiDriver.BgApi;

namespace BgApiApp
{
    public class BlueGigaService : BaseBlueGiga
    {
        private ble_msg_attclient_group_found_evt_t _attClientGroupFoundEvent;
        private List<BlueGigaCharacteristic> _characteristics;
        private BlueGigaBleAdapter _adapter;

        public BlueGigaService(BlueGigaBleAdapter adapter, ble_msg_attclient_group_found_evt_t attClientGroupFoundEvent)
        {
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            _attClientGroupFoundEvent = attClientGroupFoundEvent ?? throw new ArgumentNullException(nameof(attClientGroupFoundEvent));

            Uuid = BitConverter.ToString(attClientGroupFoundEvent.uuid.ToArray().Reverse().ToArray()).Replace("-", "");
            _characteristics = new List<BlueGigaCharacteristic>();
        }

        public string Uuid { get; private set; }

        public IReadOnlyCollection<BlueGigaCharacteristic> GetGattCharacteristics()
        {
            var response = ExecuteOperation(() =>
            {
                return _adapter.ble_cmd_attclient_read_by_type(
                    _attClientGroupFoundEvent.connection,
                    _attClientGroupFoundEvent.start,
                    _attClientGroupFoundEvent.end, new byte[] { 03, 0x28 });
            });

            _characteristics.Clear();

            _adapter.WaitForEvent((evt) =>
            {
                if (evt is ble_msg_attclient_attribute_value_evt_t attClientAttributeValueEvent &&
                    attClientAttributeValueEvent.connection == _attClientGroupFoundEvent.connection)
                {
                    _characteristics.Add(new BlueGigaCharacteristic(_adapter, attClientAttributeValueEvent));
                    return EventProcessingResult.Processed;
                }
                else if (evt is ble_msg_attclient_procedure_completed_evt_t attClientProcedureCompleteEvent &&
                        attClientProcedureCompleteEvent.connection == _attClientGroupFoundEvent.connection)
                {
                    return EventProcessingResult.Complete;
                }
                else
                {
                    return EventProcessingResult.Skip;
                }
            });

            return _characteristics;
        }
    }
}