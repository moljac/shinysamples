using System;
using System.Threading.Tasks;
using Shiny.Push;
using Samples.Models;


namespace Samples.ShinyDelegates
{
    public class PushDelegate : IPushDelegate
    {
        readonly CoreDelegateServices services;
        readonly IPushManager pushManager;


        public PushDelegate(CoreDelegateServices services, IPushManager pushManager)
        {
            this.services = services;
            this.pushManager = pushManager;
        }


        public async Task OnReceived(IPushNotification notification)
        {
            var msg = notification.Body ?? "No Message";
            var n = this.services.Notifications;
            n.Badge = notification.Badge;

            if (msg != "quiet")
            {
                await n.Send(new Shiny.Notifications.Notification
                {
                    Title = notification.Title ?? "PUSH",
                    Message = msg
                });
            }
            await this.Insert(msg);
        }


        public async Task OnTokenChanged(string token)
        {
            await this.Insert("TOKEN CHANGE");
            await services.Notifications.Send(new Shiny.Notifications.Notification
            {
                Title = "PUSH",
                Message = "Token Changed"
            });
        }


        Task Insert(string info) => this.services.Connection.InsertAsync(new PushEvent
        {
            Payload = info,
            Token = this.pushManager.CurrentRegistrationToken,
            Timestamp = DateTime.UtcNow
        });
    }
}
