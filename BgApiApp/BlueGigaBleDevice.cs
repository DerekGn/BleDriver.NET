using System;
using BgApiDriver;

namespace BgApiApp
{
    public class BlueGigaBleDevice
    {
        private BlueGigaBleAdapter _blueGigaBleAdapter;
        private BlueGigaBleAdvertisement _advertisement;
        private int _connectioHandle;

        public BlueGigaBleDevice(BlueGigaBleAdapter adapter, BlueGigaBleAdvertisement advertisement)
        {
            _blueGigaBleAdapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            _advertisement = advertisement ?? throw new ArgumentNullException(nameof(advertisement));
            _connectioHandle = -1;
        }

        public void Connect()
        {
            var response = _blueGigaBleAdapter.ble_cmd_gap_connect_direct(
                _advertisement.ScanResponseEvent.sender,
                _advertisement.ScanResponseEvent.address_type,
                60, // 60 * 1.25ms
                76, // 72 * 1.25ms
                100, // 100 * 10ms
                0);

            if(response.result != 0)
            {
                throw new BlueGigaBleException(response.result);
            }

            _connectioHandle = response.connection_handle;

#warning TODO wait for event signal
            //_blueGigaBleAdapter.WaitForEvent();
        }

        //public IList<BlueGigaService> GetGattServices()
        //{
        //    var response = _blueGigaBleAdapter.ble_cmd_attclient_read_by_group_type(_connectioHandle, 0x1, int.MaxValue, new byte[] { 0, 0x28});


        //}

        public void Disconnect()
        {
            var response = _blueGigaBleAdapter.ble_cmd_connection_disconnect(_connectioHandle);

            if (response.result != 0)
            {
                throw new BlueGigaBleException(response.result);
            }
#warning TODO wait for event signal
            //_blueGigaBleAdapter.WaitForEvent();
        }

        private void ExecuteOperation(Func<BgApiResponse> operation, Action<BgApiResponse> processResponse)
        {
            var response = operation();

            if(response.result != 0)
            {
                throw new BlueGigaBleException(response.result);
            }

//            _blueGigaBleAdapter.WaitForCompletion(response.Id);
        }
    }
}