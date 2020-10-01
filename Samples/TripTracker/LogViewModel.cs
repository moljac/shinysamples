using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Shiny;
using Shiny.TripTracker;
using Samples.Infrastructure;


namespace Samples.TripTracker
{
    public class LogViewModel : AbstractLogViewModel<CommandItem>
    {
        readonly ITripTrackerManager manager;
        public LogViewModel(ITripTrackerManager manager, IDialogs dialogs) : base(dialogs)
            => this.manager = manager;


        protected override Task ClearLogs() => this.manager.Purge();


        protected override async Task<IEnumerable<CommandItem>> LoadLogs()
        {
            var trips = await this.manager.GetAllTrips();

            return trips.Select(x =>
            {
                var km = Distance.FromMeters(x.TotalDistanceMeters).TotalKilometers;

                var text = String.Empty;
                if (x.DateFinished == null)
                {
                    text = $"UNFINISHED: {x.DateStarted.LocalDateTime:g}";
                }
                else
                {
                    var format = x.DateStarted.Date == x.DateFinished.Value.Date ? "t" : "g";
                    text = $"FINISHED: {x.DateStarted.LocalDateTime:g} - {x.DateFinished.Value.LocalDateTime.ToString(format)}";
                }

                return new CommandItem
                {
                    Text = text,
                    Detail = $"Distance: {km} km",
                    PrimaryCommand = ReactiveCommand.CreateFromTask(async () =>
                    {
                        var email = await this.Dialogs.Input("Do you wish to email this trip?  If so, enter and ok it!");
                        if (email == null)
                            return;

                        var sb = new StringBuilder();
                        var checkins = await this.manager.GetCheckinsByTrip(x.Id);
                        sb
                            .AppendLine($"Trip: {x.Id}")
                            .AppendLine()
                            .AppendLine($"Started: {x.DateStarted.LocalDateTime}")
                            .AppendLine($"Start Location: {x.StartLatitude} / {x.StartLongitude}")
                            .AppendLine();

                        if (x.DateFinished != null)
                        {
                            sb
                                .AppendLine($"Finished: {x.DateFinished.Value.LocalDateTime:g}")
                                .AppendLine($"Finish Location: {x.EndLatitude} / {x.EndLongitude}")
                                .AppendLine();
                        }
                        sb
                            .AppendLine($"Total Distance (meters): {x.TotalDistanceMeters}")
                            .AppendLine($"GPS Pings: {checkins.Count()}")
                            .AppendLine()
                            .AppendLine("Lat,Long,Speed,Direction,Ticks");

                        foreach (var checkin in checkins)
                            sb.AppendLine($"{checkin.Latitude},{checkin.Longitude},{checkin.Speed},{checkin.Direction},{checkin.DateCreated.Ticks}");

                        await Xamarin.Essentials.Email.ComposeAsync("Shiny Trip", sb.ToString(), email);
                    })
                };
            });
        }
    }
}
