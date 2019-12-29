using Shiny.Push;
using System;
using System.Threading.Tasks;


namespace Samples.ShinyDelegates
{
    public class PushNotificationDelegate : IPushNotificationDelegate
    {
        readonly CoreDelegateServices services;
        public PushNotificationDelegate(CoreDelegateServices services) => this.services = services;


        public Task OnReceived(string payload)
        {
            return Task.CompletedTask;
        }
    }
}
