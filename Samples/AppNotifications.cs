using System;
using System.Threading.Tasks;
using Shiny.Notifications;
using Shiny.Settings;


namespace Samples
{
    public class AppNotifications
    {
        readonly ISettings settings;
        readonly INotificationManager notifications;


        public AppNotifications(ISettings settings, INotificationManager notifications)
        {
            this.settings = settings;
            this.notifications = notifications;
        }


        public void Set(Type type, bool entry, bool enabled)
            => this.settings.Set(ToKey(type, entry), enabled);


        //public void Register(Type type, bool hasEntryExit, string description)
        //{

        //}


        public async Task Send(Type type, bool entry, string title, string message)
        {
            var enabled = this.settings.Get(ToKey(type, entry), true);
            if (enabled)
                await this.notifications.Send(title, message);
        }


        static string ToKey(Type type, bool entry)
        {
            var e = entry ? "Entry" : "Exit";
            var key = $"{type.FullName}.{e}";
            return key;
        }
    }
}
