using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;
using Shiny.BluetoothLE.Central;


namespace Samples.BluetoothLE
{
    public class CentralExtensionsViewModel : ViewModel
    {
        public CentralExtensionsViewModel(ICentralManager centralManager)
        {
            //centralManager.ScanInterval
            //centralManager.ScanForUniquePeripherals

            this.Tasks = new List<TaskViewModel>
            {
                new TaskViewModel(
                    "Scan Find Peripheral",
                    ct => centralManager
                        .ScanUntilPeripheralFound(this.DeviceName)
                        .ToTask(ct),

                    this.WhenAny(
                        x => x.DeviceName,
                        x => !x.GetValue().IsEmpty()
                    )
                )
            };
        }
        

        public List<TaskViewModel> Tasks { get; }
        [Reactive] public string DeviceName { get; set; }
    }
}

