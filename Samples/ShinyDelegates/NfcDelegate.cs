using Shiny.Nfc;
using System;
using System.Threading.Tasks;

namespace Samples.ShinyDelegates
{
    public class NfcDelegate : INfcDelegate
    {
        public Task OnReceived(INDefRecord[] records)
        {
            return Task.CompletedTask;
        }
    }
}
