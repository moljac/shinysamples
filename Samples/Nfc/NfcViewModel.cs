using Shiny.Nfc;
using System;


namespace Samples.Nfc
{
    public class NfcViewModel : ViewModel
    {
        readonly INfcManager nfcManager;


        public NfcViewModel(SampleSqliteConnection conn, INfcManager nfcManager = null)
        {
            this.nfcManager = nfcManager;
        }
    }
}
