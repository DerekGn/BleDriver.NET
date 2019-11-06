using Serilog;
using System;
using System.Linq;
using System.Threading;
using static BgApiDriver.BgApi;

namespace BgApiApp
{
    class Program
    {
        private static ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Information()
               .WriteTo.Console()
               .CreateLogger();

            try
            {
                BlueGigaBleAdapter bled112 = new BlueGigaBleAdapter("COM3");
                bled112.Added += BleDeviceAdded;
                bled112.Updated += Bled112Updated;

                bled112.Open();

                bled112.ble_cmd_gap_set_scan_parameters(0x4B, 0x32, 1);

                bled112.ble_cmd_gap_discover((int) gap_discover_mode.gap_discover_observation);

                _manualResetEvent.WaitOne();

                //var bled112Device = bled112.CreateBleDevice(bled112.Advertisements.First());

                Thread.Sleep(TimeSpan.FromMinutes(1));

                bled112.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void Bled112Updated(object sender, BleDeviceEventArgs e)
        {
            Log.Information($"Device Updated Address: [{e.Advertisement.Address}] Name: [{e.Advertisement.Name}] Rssi: [{e.Advertisement.Rssi}]");

            if(e.Advertisement.Name.StartsWith("Eko"))
            {
                _manualResetEvent.Set();
            }
        }

        private static void BleDeviceAdded(object sender, BleDeviceEventArgs e)
        {
            Log.Information($"Device Added Address: [{e.Advertisement.Address}] Name: [{e.Advertisement.Name}]  Rssi: [{e.Advertisement.Rssi}]");
        }
    }
}
