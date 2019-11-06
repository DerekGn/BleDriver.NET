using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BgApiDriver;
using static BgApiDriver.BgApi;

namespace BgApiApp
{
    public class BlueGigaBleAdvertisement
    {
        public BlueGigaBleAdvertisement(ble_msg_gap_scan_response_evt_t scanResponse)
        {
            Services = new List<string>();

            ParseEvent(scanResponse);
        }

        public int Rssi { get; private set; }

        public PacketType PacketType { get; private set; }

        public string Address { get; private set; }

        public AddressType AddressType { get; private set; }

        public int Bond { get; private set; }

        public string Name { get; private set; }

        public double MaxConnectionInterval { get; private set; }

        public double MinConnectionInterval { get; private set; }

        public byte TxPower { get; private set; }

        public GapAdvertisementFlags Flags { get; private set; }

        public List<string> Services { get; private set; }

        public byte[] ManufacturerSpecificData { get; private set; }

        internal DateTime Timestamp { get; private set; }

        internal void Update(ble_msg_gap_scan_response_evt_t arg)
        {
            Timestamp = DateTime.UtcNow;

            ParseEvent(arg);
        }

        private void ParseAdvertisementData(byte[] advertisementData)
        {
            for (byte i = 0; i < advertisementData.Length;)
            {
                int length = advertisementData[i] - 1;
                byte type = advertisementData[i + 1];
                switch (type)
                {
                    case 0x01://Flags
                        Flags = (GapAdvertisementFlags) advertisementData.Skip(i + 2).Take(length).First();
                        break;
                    case 0x02://Incomplete List of 16-bit Service Class UUIDs
                        for (byte j = 0; j < length; j += 2)
                        {
                            Services.Add(BitConverter.ToString(advertisementData.Skip(i + j + 2).Take(2).Reverse().ToArray()).Replace("-", ""));
                        }
                        break;
                    case 0x03://Complete List of 16-bit Service Class UUIDs
                    case 0x07://Complete List of 128-bit Service Class UUIDs
                        Services.Add(BitConverter.ToString(advertisementData.Skip(i + 2).Take(length).Reverse().ToArray()).Replace("-", ""));
                        break;
                    case 0x08://Shortened Local Name
                    case 0x09://Complete Local Name
                        Name = Encoding.UTF8.GetString(advertisementData.Skip(i + 2).Take(length).ToArray()).Trim('\0');
                        break;
                    case 0x12://Slave Connection Interval Range
                        byte[] connectionInterval = advertisementData.Skip(i + 2).Take(length).ToArray();
                        MaxConnectionInterval = 1.25 * BitConverter.ToUInt16(connectionInterval, 2);
                        MinConnectionInterval = 1.25 * BitConverter.ToUInt16(connectionInterval, 0);
                        break;
                    case 0x0A://Tx Power Level
                        TxPower = advertisementData[i + 2];
                        break;
                    case 0xFF://Manufacturer Specific Data
                        ManufacturerSpecificData = advertisementData.Skip(i + 2).Take(length).ToArray();
                        break;
                    default:
                        break;
                }
                i += (byte)(advertisementData[i] + 1);
            }
        }

        private void ParseEvent(ble_msg_gap_scan_response_evt_t scanResponse)
        {
            Rssi = scanResponse.rssi;
            PacketType = (PacketType)scanResponse.packet_type;
            Address = BitConverter.ToString(scanResponse.sender.Address.ToArray().Reverse().ToArray()).Replace('-', ':');
            AddressType = (AddressType)scanResponse.address_type;
            Bond = scanResponse.bond;
            ParseAdvertisementData(scanResponse.data);
        }
    }
}
