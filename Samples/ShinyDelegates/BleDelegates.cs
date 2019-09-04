using System;
using System.Threading.Tasks;
using Samples.Models;
using Shiny;
using Shiny.BluetoothLE;
using Shiny.BluetoothLE.Central;


namespace Samples.ShinyDelegates
{
    public class BleDelegates : IBleAdapterDelegate, IBlePeripheralDelegate
    {
        readonly CoreDelegateServices services;
        public BleDelegates(CoreDelegateServices services) => this.services = services;


        public async Task OnBleAdapterStateChanged(AccessState state)
        {
            if (state != AccessState.Available && this.services.AppSettings.UseNotificationsBle)
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
