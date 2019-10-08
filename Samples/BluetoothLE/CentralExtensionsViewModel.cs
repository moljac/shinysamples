using System;
using System.Windows.Input;
using Acr.UserDialogs.Forms;
using Shiny.BluetoothLE.Central;


namespace Samples.BluetoothLE
{
    public class CentralExtensionsViewModel : ViewModel
    {
        public CentralExtensionsViewModel(ICentralManager centralManager, IUserDialogs dialogs)
        {
        }


        public ICommand ScanForDevice { get; }
    }
}

