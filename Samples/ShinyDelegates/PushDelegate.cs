using Samples.Models;
using Shiny.Push;
using System;
using System.Threading.Tasks;


namespace Samples.ShinyDelegates
{
    public class PushDelegate : IPushDelegate
    {
        readonly CoreDelegateServices services;
        public PushDelegate(CoreDelegateServices services) => this.services = services;


        public Task OnReceived(string payload) => this.services.Connection.InsertAsync(new PushEvent
        {
            // TODO: save token?
            Payload = payload,
            Timestamp = DateTime.UtcNow
        });
    }
}
