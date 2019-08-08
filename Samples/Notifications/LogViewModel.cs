using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs.Forms;
using Samples.Infrastructure;
using Samples.Models;


namespace Samples.Notifications
{
    public class LogViewModel : AbstractLogViewModel<CommandItem>
    {
        readonly SampleSqliteConnection conn;
        public LogViewModel(SampleSqliteConnection conn, IUserDialogs dialogs) : base(dialogs)
        {
            this.conn = conn;
        }


        protected override Task ClearLogs() => this.conn.DeleteAllAsync<NotificationEvent>();


        protected override async Task<IEnumerable<CommandItem>> LoadLogs()
        {
            var events = await conn
                .NotificationEvents
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();

            return events.Select(x => new CommandItem
            {
                Text = $"({x.NotificationId}) {x.NotificationTitle}",
                Detail = $"{x.IsEntry} {x.Timestamp}"
            });
        }
    }
}
