namespace BgApiApp
{
    public class BleDeviceEventArgs
    {
        public BlueGigaAdvertisement Advertisement { get; private set; }

        public BleDeviceEventArgs(BlueGigaAdvertisement blueGigaBleAdvertisement)
        {
            Advertisement = blueGigaBleAdvertisement;
        }
    }
}