using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using BgApiApp.Exceptions;
using BgApiDriver;

using Serilog;

namespace BgApiApp
{
    public class BlueGigaBleAdapter : BgApi
    {
        private BlockingCollection<BgApiEvent> _events;

        public BlueGigaBleAdapter(string port) : base(port)
        {
            Advertisements = new List<BlueGigaAdvertisement>();

            _events = new BlockingCollection<BgApiEvent>();
        }

        public event EventHandler<BleDeviceEventArgs> Added;

        public event EventHandler<BleDeviceEventArgs> Removed;

        public event EventHandler<BleDeviceEventArgs> Updated;

        public IList<BlueGigaAdvertisement> Advertisements { get; private set; }

        public override void Close()
        {
            base.Close();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void Open()
        {
            base.Open();
        }

        public BlueGigaDevice CreateBleDevice(BlueGigaAdvertisement advertisement)
        {
            return new BlueGigaDevice(this, advertisement);
        }

        internal void WaitForEvent(Func<BgApiEvent, EventProcessingResult> processEvent)
        {
            do
            {
                if(_events.TryTake(out var evt, 1000))
                {
                    var result = processEvent(evt);

                    if(result == EventProcessingResult.Complete)
                    {
                        break;
                    }

                    if (result == EventProcessingResult.Skip)
                    {
                        _events.Add(evt);
                    }
                }
                else
                {
                    throw new BlueGigaBleTimeoutException();
                }
            } while (true);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override void ble_evt_attclient_attribute_found(ble_msg_attclient_attribute_found_evt_t arg)
        {
            base.ble_evt_attclient_attribute_found(arg);
        }

        protected override void ble_evt_attclient_attribute_value(ble_msg_attclient_attribute_value_evt_t arg)
        {
            base.ble_evt_attclient_attribute_value(arg);

            _events.Add(arg);
        }

        protected override void ble_evt_attclient_find_information_found(ble_msg_attclient_find_information_found_evt_t arg)
        {
            base.ble_evt_attclient_find_information_found(arg);
        }

        protected override void ble_evt_attclient_group_found(ble_msg_attclient_group_found_evt_t arg)
        {
            base.ble_evt_attclient_group_found(arg);

            _events.Add(arg);
        }

        protected override void ble_evt_attclient_indicated(ble_msg_attclient_indicated_evt_t arg)
        {
            base.ble_evt_attclient_indicated(arg);
        }

        protected override void ble_evt_attclient_procedure_completed(ble_msg_attclient_procedure_completed_evt_t arg)
        {
            base.ble_evt_attclient_procedure_completed(arg);

            _events.Add(arg);
        }

        protected override void ble_evt_attclient_read_multiple_response(ble_msg_attclient_read_multiple_response_evt_t arg)
        {
            base.ble_evt_attclient_read_multiple_response(arg);
        }

        protected override void ble_evt_attributes_status(ble_msg_attributes_status_evt_t arg)
        {
            base.ble_evt_attributes_status(arg);
        }

        protected override void ble_evt_attributes_user_read_request(ble_msg_attributes_user_read_request_evt_t arg)
        {
            base.ble_evt_attributes_user_read_request(arg);
        }

        protected override void ble_evt_attributes_value(ble_msg_attributes_value_evt_t arg)
        {
            base.ble_evt_attributes_value(arg);
        }

        protected override void ble_evt_connection_disconnected(ble_msg_connection_disconnected_evt_t arg)
        {
            base.ble_evt_connection_disconnected(arg);

            _events.Add(arg);
        }

        protected override void ble_evt_connection_feature_ind(ble_msg_connection_feature_ind_evt_t arg)
        {
            base.ble_evt_connection_feature_ind(arg);
        }

        protected override void ble_evt_connection_raw_rx(ble_msg_connection_raw_rx_evt_t arg)
        {
            base.ble_evt_connection_raw_rx(arg);
        }

        protected override void ble_evt_connection_status(ble_msg_connection_status_evt_t arg)
        {
            base.ble_evt_connection_status(arg);

            _events.Add(arg);
        }

        protected override void ble_evt_connection_version_ind(ble_msg_connection_version_ind_evt_t arg)
        {
            base.ble_evt_connection_version_ind(arg);
        }

        protected override void ble_evt_dfu_boot(ble_msg_dfu_boot_evt_t arg)
        {
            base.ble_evt_dfu_boot(arg);
        }

        protected override void ble_evt_flash_ps_key(ble_msg_flash_ps_key_evt_t arg)
        {
            base.ble_evt_flash_ps_key(arg);
        }

        protected override void ble_evt_gap_mode_changed(ble_msg_gap_mode_changed_evt_t arg)
        {
            base.ble_evt_gap_mode_changed(arg);
        }

        protected override void ble_evt_gap_scan_response(ble_msg_gap_scan_response_evt_t arg)
        {
            var advertisement = Advertisements.FirstOrDefault(a => a.Address == arg.sender.GetValue());

            if (advertisement != null)
            {
                advertisement.Update(arg);

                OnAdvertisementUpdated(advertisement);
            }
            else
            {
                advertisement = new BlueGigaAdvertisement(arg);

                Advertisements.Add(advertisement);

                OnAdvertisementAdded(advertisement);
            }
        }

        protected override void ble_evt_hardware_adc_result(ble_msg_hardware_adc_result_evt_t arg)
        {
            base.ble_evt_hardware_adc_result(arg);
        }

        protected override void ble_evt_hardware_analog_comparator_status(ble_msg_hardware_analog_comparator_status_evt_t arg)
        {
            base.ble_evt_hardware_analog_comparator_status(arg);
        }

        protected override void ble_evt_hardware_io_port_status(ble_msg_hardware_io_port_status_evt_t arg)
        {
            base.ble_evt_hardware_io_port_status(arg);
        }

        protected override void ble_evt_hardware_soft_timer(ble_msg_hardware_soft_timer_evt_t arg)
        {
            base.ble_evt_hardware_soft_timer(arg);
        }

        protected override void ble_evt_sm_bonding_fail(ble_msg_sm_bonding_fail_evt_t arg)
        {
            base.ble_evt_sm_bonding_fail(arg);
        }

        protected override void ble_evt_sm_bond_status(ble_msg_sm_bond_status_evt_t arg)
        {
            base.ble_evt_sm_bond_status(arg);
        }

        protected override void ble_evt_sm_passkey_display(ble_msg_sm_passkey_display_evt_t arg)
        {
            base.ble_evt_sm_passkey_display(arg);
        }

        protected override void ble_evt_sm_passkey_request(ble_msg_sm_passkey_request_evt_t arg)
        {
            base.ble_evt_sm_passkey_request(arg);
        }

        protected override void ble_evt_sm_smp_data(ble_msg_sm_smp_data_evt_t arg)
        {
            base.ble_evt_sm_smp_data(arg);
        }

        protected override void ble_evt_system_boot(ble_msg_system_boot_evt_t arg)
        {
            base.ble_evt_system_boot(arg);
        }

        protected override void ble_evt_system_debug(ble_msg_system_debug_evt_t arg)
        {
            base.ble_evt_system_debug(arg);
        }

        protected override void ble_evt_system_endpoint_watermark_rx(ble_msg_system_endpoint_watermark_rx_evt_t arg)
        {
            base.ble_evt_system_endpoint_watermark_rx(arg);
        }

        protected override void ble_evt_system_endpoint_watermark_tx(ble_msg_system_endpoint_watermark_tx_evt_t arg)
        {
            base.ble_evt_system_endpoint_watermark_tx(arg);
        }

        protected override void ble_evt_system_no_license_key(ble_msg_system_no_license_key_evt_t arg)
        {
            base.ble_evt_system_no_license_key(arg);
        }

        protected override void ble_evt_system_protocol_error(ble_msg_system_protocol_error_evt_t arg)
        {
            base.ble_evt_system_protocol_error(arg);
        }

        protected override void ble_evt_system_script_failure(ble_msg_system_script_failure_evt_t arg)
        {
            base.ble_evt_system_script_failure(arg);
        }

        protected override void HandleEvent(BgApiEvent evt)
        {
            base.HandleEvent(evt);
        }

        protected override void log(string msg)
        {
            Log.Debug(msg);
        }

        private void OnAdvertisementAdded(BlueGigaAdvertisement advertisement)
        {
            Added?.Invoke(this, new BleDeviceEventArgs(advertisement));
        }

        private void OnAdvertisementUpdated(BlueGigaAdvertisement advertisement)
        {
            Updated?.Invoke(this, new BleDeviceEventArgs(advertisement));
        }
    }
}
