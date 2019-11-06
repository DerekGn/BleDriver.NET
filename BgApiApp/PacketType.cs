using System;

namespace BgApiApp
{
    [Flags]
    public enum PacketType
    {
        ConnectableAdvertisement = 0,
        NonConnectableAdvertisement = 2,
        ScanResponse = 4,
        DiscoverableAdvertisement = 6,
    }
}