namespace BgApiApp
{
    public class BleDeviceEventArgs
    {
        public BlueGigaBleAdvertisement Advertisement { get; private set; }

        public BleDeviceEventArgs(BlueGigaBleAdvertisement blueGigaBleAdvertisement)
        {
            Advertisement = blueGigaBleAdvertisement;
        }
    }
}