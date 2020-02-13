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


        public Task OnReceived(IPushNotification notification) => this.Pump(notification.Body);
        public Task OnTokenChanged(string token) => this.Pump("TOKEN CHANGE");
        async Task Pump(string message)
        {
            services.Notifications.Badge = 1;
            await services.Notifications.Send(new Shiny.Notifications.Notification
            {
                Title = "PUSH",
                Message = message ?? "NO MESSAGE"
            });
            await this.services.Connection.InsertAsync(new PushEvent
            {
                Payload = message ?? "NO MESSAGE",
                Token = this.pushManager.CurrentRegistrationToken,
                Timestamp = DateTime.UtcNow
            });
            this.messageBus.Publish("Push", Unit.Default);
        }
    }
}
