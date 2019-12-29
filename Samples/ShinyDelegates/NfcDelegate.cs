using Shiny.Nfc;
using System;
using System.Threading.Tasks;

namespace Samples.ShinyDelegates
{
    public class NfcDelegate : INfcDelegate
    {
        readonly CoreDelegateServices services;
        public NfcDelegate(CoreDelegateServices services) => this.services = services;

        
        public Task OnReceived(INDefRecord[] records)
        {
            return Task.CompletedTask;
        }
    }
}
