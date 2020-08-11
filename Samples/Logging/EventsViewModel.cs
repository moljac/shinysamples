using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using ReactiveUI;
using Samples.Infrastructure;
using Shiny;
using Shiny.Infrastructure;
using Shiny.Integrations.Sqlite;
using Shiny.Logging;


namespace Samples.Logging
{
    public class EventsViewModel : AbstractLogViewModel<CommandItem>
    {
        readonly ShinySqliteConnection conn;
        readonly ISerializer serializer;
        readonly INavigationService navigator;


        public EventsViewModel(ShinySqliteConnection conn,
                               ISerializer serializer,
                               IDialogs dialogs,
                               INavigationService navigator) : base(dialogs)
        {
            this.conn = conn;
            this.serializer = serializer;
            this.navigator = navigator;
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            Log
                .WhenEventLogged()
                .Select(x => new CommandItem
                {
                    Text = $"{x.EventName} ({DateTime.Now:hh:mm:ss tt})",
                    Detail = x.Description,
                    PrimaryCommand = ReactiveCommand.CreateFromTask(async () =>
                    {
                        var s = $"{x.EventName} ({DateTime.Now:hh:mm:ss tt}){Environment.NewLine}{x.Description}";
                        foreach (var p in x.Parameters)
                            s += $"{Environment.NewLine}{p.Key}: {p.Value}";

                        await this.navigator.ShowBigText(s, $"{x.EventName} ({DateTime.Now:hh:mm:ss tt})");
                    })
                })
                .SubOnMainThread(this.InsertItem)
                .DisposeWith(this.DeactivateWith);
        }


        protected override Task ClearLogs() => this.conn.Logs.DeleteAsync(x => !x.IsError);
        protected override async Task<IEnumerable<CommandItem>> LoadLogs()
        {
            var logs = await this.conn
                .Logs
                .Where(x => !x.IsError)
                .ToListAsync();

            return logs.Select(x => new CommandItem
            {
                Text = $"{x.Description} ({x.TimestampUtc.ToLocalTime():hh:mm:ss tt})",
                Detail = x.Detail,
                PrimaryCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    var s = $"{x.Description} ({x.TimestampUtc.ToLocalTime():hh:mm:ss tt}){Environment.NewLine}{x.Description}";
                    if (!x.Parameters.IsEmpty())
                    {
                        var parameters = this.serializer.Deserialize<Tuple<string, string>[]>(x.Parameters);
                        foreach (var p in parameters)
                            s += $"{Environment.NewLine}{p.Item1}: {p.Item2}";
                    }
                    await this.navigator.ShowBigText(s, $"{x.Description} ({ x.TimestampUtc.ToLocalTime():hh: mm: ss tt})");
                })
            });
        }
    }
}
