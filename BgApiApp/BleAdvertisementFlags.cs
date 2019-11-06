using System;

namespace BgApiApp
{
    [Flags]
    public enum GapAdvertisementFlags
    {
        LeLimitedDiscoverableMode = 0x01,
        LeGeneralDiscoverableMode = 0x02,
        BrEdrNotsupported = 0x04,
        SimultaneousLeAndBrEdrController = 0x8,
        SimultaneousLeAndBrEdrHost = 0x10,
        LeLimitedDiscoverableModeBrEdrNotSupported = LeLimitedDiscoverableMode | BrEdrNotsupported,
        LeGeneralDiscoverableModeBrEdrNotSupported = LeGeneralDiscoverableMode | BrEdrNotsupported
    }
}
