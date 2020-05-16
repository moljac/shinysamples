using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Samples.Infrastructure;
using Samples.Models;
using XF.Material.Forms.UI.Dialogs;


namespace Samples.Push
{
    public class EventListViewModel : AbstractLogViewModel<CommandItem>
    {
        readonly SampleSqliteConnection conn;


        public EventListViewModel(SampleSqliteConnection conn, IMaterialDialog dialogs) : base(dialogs)
        {
            this.conn = conn;
        }

        protected override Task ClearLogs() => this.conn.DeleteAllAsync<PushEvent>();
        protected override async Task<IEnumerable<CommandItem>> LoadLogs()
        {
            var data = await this.conn
                .PushEvents
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();

            return data.Select(x => new CommandItem
            {
                Text = x.Payload,
                Detail = x.Timestamp.ToLocalTime().ToString()
            });
        }
    }
}
