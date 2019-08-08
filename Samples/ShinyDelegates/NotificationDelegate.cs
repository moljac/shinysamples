using System;
using System.Threading.Tasks;
using Samples.Models;
using Shiny.Notifications;


namespace Samples.ShinyDelegates
{
    public class NotificationDelegate : INotificationDelegate
    {
        readonly SampleSqliteConnection conn;
        public NotificationDelegate(SampleSqliteConnection conn) => this.conn = conn;


        public Task OnEntry(Notification notification) => this.Store(notification, true);
        public Task OnReceived(Notification notification) => this.Store(notification, false);


        Task Store(Notification notification, bool isEntry) => this.conn.InsertAsync(new NotificationEvent
        {
            NotificationId = notification.Id,
            NotificationTitle = notification.Title ?? notification.Message,
            IsEntry = isEntry,
            Timestamp = DateTime.Now
        });
    }
}
