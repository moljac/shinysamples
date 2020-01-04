using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs.Forms;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Infrastructure;
using Samples.Models;
using Shiny;
using Shiny.Push;


namespace Samples.Push
{
    public class PushViewModel : AbstractLogViewModel<CommandItem>
    {
        readonly SampleSqliteConnection conn;


        public PushViewModel(SampleSqliteConnection conn,
                             IUserDialogs dialogs,
                             IPushManager pushManager = null) : base(dialogs)
        {
            this.conn = conn;

            this.CheckPermission = ReactiveCommand.CreateFromTask(async () =>
            {
                var status = await pushManager.RequestAccess();
                this.AccessStatus = status.Status;
                this.RegToken = status.RegistrationToken;
            });
        }


        public ICommand CheckPermission { get; }
        [Reactive] public string RegToken { get; private set; }
        [Reactive] public AccessState AccessStatus { get; private set; }

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
