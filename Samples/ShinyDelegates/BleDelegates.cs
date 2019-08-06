using System;
using Shiny;
using Shiny.BluetoothLE;
using Shiny.BluetoothLE.Central;


namespace Samples.ShinyDelegates
{
    public class BleDelegates : IBleAdapterDelegate, IBlePeripheralDelegate
    {
        readonly CoreDelegateServices services;
        public BleDelegates(CoreDelegateServices services) => this.services = services;


        public async void OnBleAdapterStateChanged(AccessState state)
        {
            if (state != AccessState.Available && this.services.AppSettings.UseNotificationsBle)
                await this.services.SendNotification("BLE State", "Turn on Bluetooth already");
        }


        public void OnConnected(IPeripheral peripheral)
        {
            //await this.DoNotification(
            //    "BluetoothLE Device Connected",
            //    $"{region.Identifier} was {newStatus}",
            //    this.appSettings.UseBleNotifications
            //);
        }
    }
}
