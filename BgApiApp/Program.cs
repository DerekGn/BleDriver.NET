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

                Console.WriteLine($"Scan result [{bled112.ble_cmd_gap_set_scan_parameters(0x4B, 0x32, 1).result}]");

                Console.WriteLine($"Discover result [{bled112.ble_cmd_gap_discover((int)gap_discover_mode.gap_discover_observation).result}]");

                _manualResetEvent.WaitOne();

                var advertisement = bled112.Advertisements.First(a => a.Name.StartsWith("Eko"));

                Log.Information($"Connecting to [{advertisement.Address}] [{advertisement.Name}]");

                var bled112Device = bled112.CreateBleDevice(advertisement);

                bled112Device.Connect();

                var services = bled112Device.GetGattServices();

                Log.Information($"Services Count: [{services.Count}]");

                foreach (var service in services)
                {
                    Log.Information($"Service UUID: [{service.Uuid}]");
                }

                var batteryService = services.FirstOrDefault(s => s.Uuid == "180F");

                if(batteryService == null)
                {
                    Log.Information("Battery Service not found");
                }
                else
                {
                    var characteristics = batteryService.GetGattCharacteristics();

                    var batteryCharacteristic = characteristics.FirstOrDefault(s => s.Uuid == "");

                    if(batteryCharacteristic == null)
                    {
                        Log.Information("Battery Service characteristic not found");
                    }
                    else
                    {
                        var value = batteryCharacteristic.Read();
                    }
                }

                Thread.Sleep(TimeSpan.FromSeconds(20));

                Log.Information($"Disconnecting from [{advertisement.Address}] [{advertisement.Name}]");

                bled112Device.Disconnect();

                bled112.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void Bled112Updated(object sender, BleDeviceEventArgs e)
        {
            Log.Debug($"Device Updated Address: [{e.Advertisement.Address}] Name: [{e.Advertisement.Name}] Rssi: [{e.Advertisement.Rssi}]");

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
