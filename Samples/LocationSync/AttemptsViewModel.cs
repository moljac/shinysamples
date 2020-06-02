using Samples.Infrastructure;
using Samples.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Samples.LocationSync
{
    public class AttemptsViewModel : AbstractLogViewModel<SyncAttempt>
    {
        readonly SampleSqliteConnection conn;
        public AttemptsViewModel(SampleSqliteConnection conn, IDialogs dialogs) : base(dialogs) => this.conn = conn;

        

        protected override Task ClearLogs() => this.conn.DeleteAllAsync<SyncAttempt>();
        protected override async Task<IEnumerable<SyncAttempt>> LoadLogs() => await this.conn.SyncAttempt.OrderByDescending(x => x.DateCreated).ToListAsync();
    }
}
