using System;
using System.Reactive.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs.Forms;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;
using Shiny.BluetoothLE.Central;


namespace Samples.BluetoothLE
{
    public class CentralExtensionsViewModel : ViewModel
    {
        public CentralExtensionsViewModel(ICentralManager centralManager, IUserDialogs dialogs)
        {
            this.ScanForDevice = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    await centralManager.ScanUntilPeripheralFound(this.DeviceName).ToTask();
                },
                this.WhenAny(
                    x => x.DeviceName,
                    x => !x.GetValue().IsEmpty()
                )
            );
        }


        [Reactive] public string DeviceName { get; set; }
        
        public ICommand ScanForDevice { get; }
    }
}

