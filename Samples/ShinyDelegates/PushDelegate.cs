using System;
using System.Threading.Tasks;
using System.Reactive;
using Shiny;
using Shiny.Push;
using Samples.Models;


namespace Samples.ShinyDelegates
{
    public class PushDelegate : IPushDelegate
    {
        readonly CoreDelegateServices services;
        readonly IPushManager pushManager;
        readonly IMessageBus messageBus;


        public PushDelegate(CoreDelegateServices services,
                            IPushManager pushManager,
                            IMessageBus messageBus)
        {
            this.services = services;
            this.pushManager = pushManager;
            this.messageBus = messageBus;
        }


        public Task OnReceived(string payload) => this.Pump(payload);
        public Task OnTokenChanged(string token) => this.Pump("TOKEN CHANGE");
        async Task Pump(string payload)
        {
            services.Notifications.Badge = 1;
            await services.Notifications.Send(new Shiny.Notifications.Notification
            {
                Title = "PUSH",
                Message = payload
            });
            await this.services.Connection.InsertAsync(new PushEvent
            {
                Payload = payload,
                Token = this.pushManager.CurrentRegistrationToken,
                Timestamp = DateTime.UtcNow
            });
            this.messageBus.Publish("Push", Unit.Default);
        }
    }
}
