using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Samples.Infrastructure;
using Samples.Models;


namespace Samples.MediaSync
{
    public class LogsViewModel : AbstractLogViewModel<CommandItem>
    {
        readonly SampleSqliteConnection conn;
        public LogsViewModel(SampleSqliteConnection conn, IDialogs dialogs) : base(dialogs) => this.conn = conn;
        protected override Task ClearLogs() => this.conn.DeleteAllAsync<MediaSyncEvent>();
        protected override async Task<IEnumerable<CommandItem>> LoadLogs()
        {
            var results = await this.conn
                .MediaSyncEvents
                .OrderByDescending(x => x.DateCompleted)
                .ToListAsync();

            return results.Select(x => new CommandItem
            {
                Text = x.DateCompleted.ToString(),
                Detail = x.Type,
                ImageUri = x.FilePath
            });
        }
    }
}
