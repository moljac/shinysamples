using System;
using System.Threading.Tasks;
using Samples.Infrastructure;
using Samples.Models;
using Shiny;
using Shiny.BluetoothLE;


namespace Samples.BluetoothLE
{
    public class BleClientDelegate : IBleDelegate
    {
        readonly CoreDelegateServices services;
        public BleClientDelegate(CoreDelegateServices services) => this.services = services;


        public async Task OnAdapterStateChanged(AccessState state)
        {
            if (state == AccessState.Disabled && this.services.AppSettings.UseNotificationsBle)
                await this.services.SendNotification("BLE State", "Turn on Bluetooth already");
        }


        public Task OnConnected(IPeripheral peripheral) => Task.WhenAll(
            this.services.Connection.InsertAsync(new BleEvent
            {
                Timestamp = DateTime.Now
            }),
            this.services.SendNotification(
                "BluetoothLE Device Connected",
                $"{peripheral.Name} has connected",
                x => x.UseNotificationsBle
            )
        );
    }
}
