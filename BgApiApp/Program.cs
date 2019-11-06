using Serilog;
using System;
using System.Threading;
using static BgApiDriver.BgApi;

namespace BgApiApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Console()
               .CreateLogger();

            try
            {
                BlueGigaBleAdapter bled112 = new BlueGigaBleAdapter("COM3");
                bled112.Added += BleDeviceAdded;

                bled112.Open();

                bled112.ble_cmd_gap_set_scan_parameters(0x4B00, 0x3200, 1);

                bled112.ble_cmd_gap_discover((int) gap_discover_mode.gap_discover_observation);

                Thread.Sleep(TimeSpan.FromMinutes(1));

                bled112.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void BleDeviceAdded(object sender, BleDeviceEventArgs e)
        {
            Log.Information($"Device Added Name: [{e.Advertisement.Name}]");
        }
    }
}
