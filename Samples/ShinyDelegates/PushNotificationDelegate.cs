using Shiny.Push;
using System;
using System.Threading.Tasks;


namespace Samples.ShinyDelegates
{
    public class PushNotificationDelegate : IPushNotificationDelegate
    {
        public Task OnReceived(string payload)
        {
            return Task.CompletedTask;
        }
    }
}
