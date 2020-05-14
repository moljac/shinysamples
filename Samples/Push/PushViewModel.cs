using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Infrastructure;
using Samples.Models;
using Shiny;
using Shiny.Push;
using XF.Material.Forms.UI.Dialogs;


namespace Samples.Push
{
    public class PushViewModel : AbstractLogViewModel<CommandItem>
    {
        readonly SampleSqliteConnection conn;
        readonly IMaterialDialog dialogs;
        readonly IPushManager? pushManager;


        public PushViewModel(SampleSqliteConnection conn,
                             IMaterialDialog dialogs,
                             IPushManager? pushManager = null) : base(dialogs)
        {
            this.conn = conn;
            this.dialogs = dialogs;
            this.pushManager = pushManager;

            this.CheckPermission = this.Create(async () =>
            {
                var status = await pushManager.RequestAccess();
                this.AccessStatus = status.Status;
                this.RegToken = status.RegistrationToken;
            });
            this.UnRegister = this.Create(async () =>
            {
                await pushManager.UnRegister();
                this.AccessStatus = AccessState.Disabled;
                this.RegToken = String.Empty;
            });
        }


        public override async void OnAppearing()
        {
            base.OnAppearing();
            if (this.pushManager == null)
            {
                await this.dialogs.AlertAsync("Push not supported");
            }
            else
            {
                this.pushManager
                    .WhenReceived()
                    .SubOnMainThread(_ => ((ICommand)this.Load).Execute(null))
                    .DisposedBy(this.DeactivateWith);
            }
        }


        public ICommand CheckPermission { get; }
        public ICommand UnRegister { get; }
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


        ICommand Create(Func<Task> create) => ReactiveCommand.CreateFromTask(async () =>
        {
            if (this.pushManager == null)
            {
                await this.dialogs.AlertAsync("Push not supported");
                return;
            }
            await create();
        });
    }
}
