using Samples.Models;
using Shiny.Nfc;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Samples.ShinyDelegates
{
    public class NfcDelegate : INfcDelegate
    {
        readonly CoreDelegateServices services;
        public NfcDelegate(CoreDelegateServices services) => this.services = services;

        
        public async Task OnReceived(INDefRecord[] records) 
        {
            var data = records
                .Select(x => new NfcEvent 
                {
                    Identifier = x.Identifier,
                    Payload = x.Payload,
                    PayloadType = x.PayloadType,
                    Uri = x.Uri,
                    Timestamp = DateTime.UtcNow
                })
                .ToList();

            await this.services.Connection.InsertAllAsync(data);
        }
    }
}
