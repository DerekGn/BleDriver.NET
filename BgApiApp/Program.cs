﻿using BgApiApp.Exceptions;
using Serilog;
using System;
using System.Collections.Generic;
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

            BlueGigaBleAdapter bled112 = null;

            try
            {
                bled112 = new BlueGigaBleAdapter("COM3");
                bled112.Added += BleDeviceAdded;
                bled112.Updated += Bled112Updated;

                bled112.Open();

                Console.WriteLine($"Scan result [{bled112.ble_cmd_gap_set_scan_parameters(0x4B, 0x32, 1).result}]");

                Console.WriteLine($"Discover result [{bled112.ble_cmd_gap_discover((int)gap_discover_mode.gap_discover_observation).result}]");

                ConnectAndRead(bled112);

                //Console.WriteLine($"Discover result [{bled112.ble_cmd_gap_discover((int)gap_discover_mode.gap_discover_observation).result}]");

                //_manualResetEvent.Reset();

                //ConnectAndRead(bled112);
            }
            catch(BlueGigaBleException ex)
            {
                Console.WriteLine($"Result: {ex.Result:X}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if(bled112 != null)
                {
                    bled112.Close();
                }
            }
        }

        private static void ConnectAndRead(BlueGigaBleAdapter bled112)
        {
            BlueGigaDevice bled112Device = null;

            try
            {
                _manualResetEvent.WaitOne();

                var advertisement = bled112.Advertisements.First(a => a.Name.StartsWith("Eko"));

                Log.Information($"Connecting to [{advertisement.Address}] [{advertisement.Name}]");

                bled112Device = bled112.CreateBleDevice(advertisement);

                bled112Device.Connect();

                bled112Device.EncryptConnection();

                var services = bled112Device.GetGattServices();

                Log.Information($"Services Count: [{services.Count}]");

                foreach (var service in services)
                {
                    Log.Information($"Service UUID: [{service.Uuid}]");
                }

                ReadBattery(services);

                var dataService = services.FirstOrDefault(s => s.Uuid == "5BF6E500999911E3A1160002A5D5C51B");

                if(dataService != null)
                {
                    var dataCharacteristic = dataService.GetGattCharacteristics().FirstOrDefault(c => c.Uuid == "BA9C5360999911E3966F0002A5D5C51B");

                    if (dataCharacteristic != null)
                    {
                        dataCharacteristic.EnableNotificationAsync();

                        Thread.Sleep(TimeSpan.FromSeconds(20000));

                        dataCharacteristic.DisableNotificationAsync();
                    }
                    else
                    {
                        Console.WriteLine("Unable to find data Characteristic");
                    }
                }
                else
                {
                    Console.WriteLine("Unable to find data service");
                }

                Log.Information($"Disconnecting from [{advertisement.Address}] [{advertisement.Name}]");
            }
            finally
            {
                if (bled112Device != null)
                {
                    bled112Device.Disconnect();
                }
            }
        }

        private static void SubscribeData(IReadOnlyCollection<BlueGigaService> services)
        {
            throw new NotImplementedException();
        }

        private static void ReadBattery(IReadOnlyCollection<BlueGigaService> services)
        {
            var batteryService = services.FirstOrDefault(s => s.Uuid == "180F");

            if (batteryService == null)
            {
                Log.Information("Battery Service not found");
            }
            else
            {
                var characteristics = batteryService.GetGattCharacteristics();

                var batteryCharacteristic = characteristics.FirstOrDefault(s => s.Uuid == "2A19");

                if (batteryCharacteristic == null)
                {
                    Log.Information("Battery Service characteristic not found");
                }
                else
                {
                    var value = batteryCharacteristic.Read();

                    Log.Information($"Value: [{BitConverter.ToInt16(value.ToArray(), 0)}]");
                }
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
