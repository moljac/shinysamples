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
    public class ErrorLogViewModel : AbstractLogViewModel<CommandItem>
    {
        readonly ShinySqliteConnection conn;
        readonly ISerializer serializer;
        readonly INavigationService navigator;


        public ErrorLogViewModel(ShinySqliteConnection conn,
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
                .WhenExceptionLogged()
                .Select(x => new CommandItem
                {
                    Text = DateTime.Now.ToString(),
                    Detail = x.Exception.ToString(),
                    PrimaryCommand = ReactiveCommand.CreateFromTask(async () =>
                    {
                        var s = $"{x.Exception}{Environment.NewLine}";
                        foreach (var p in x.Parameters)
                            s += $"{Environment.NewLine}{p.Key}: {p.Value}";

                        await navigator.ShowBigText(s, $"Error: {DateTime.Now}");
                    })
                })
                .SubOnMainThread(this.InsertItem)
                .DisposeWith(this.DeactivateWith);
        }

        protected override Task ClearLogs() => this.conn.Logs.DeleteAsync(x => x.IsError);
        protected override async Task<IEnumerable<CommandItem>> LoadLogs()
        {
            var results = await this.conn
                .Logs
                .Where(x => x.IsError)
                .OrderByDescending(x => x.TimestampUtc)
                .ToListAsync();

            return results.Select(x => new CommandItem
            {
                Text = x.TimestampUtc.ToString(),
                Detail = x.Description,
                PrimaryCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    var s = $"{x.TimestampUtc.ToLocalTime()}{Environment.NewLine}{x.Description}{Environment.NewLine}";
                    if (!x.Parameters.IsEmpty())
                    {
                        var parameters = this.serializer.Deserialize<Tuple<string, string>[]>(x.Parameters);
                        foreach (var p in parameters)
                            s += $"{Environment.NewLine}{p.Item1}: {p.Item2}";
                    }
                    await navigator.ShowBigText(s, $"Error: {DateTime.Now}");
                })
            });
        }
    }
}
