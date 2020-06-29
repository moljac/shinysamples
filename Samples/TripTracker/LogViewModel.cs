using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Samples.Infrastructure;
using Shiny;
using Shiny.TripTracker;


namespace Samples.TripTracker
{
    public class LogViewModel : AbstractLogViewModel<CommandItem>
    {
        readonly ITripTrackerManager manager;


        public LogViewModel(ITripTrackerManager manager, IDialogs dialogs) : base(dialogs)
        {
            this.manager = manager;
        }


        protected override Task ClearLogs() => this.manager.Purge();


        protected override async Task<IEnumerable<CommandItem>> LoadLogs()
        {
            var trips = await this.manager.GetAllTrips();

            return trips.Select(x => 
            {
                var km = Distance.FromMeters(x.TotalDistanceMeters).TotalKilometers;
                return new CommandItem 
                {
                    Text = $"{x.DateStarted} - {x.DateFinished}",
                    Detail = $"Distance: {km}km"
                };
            });
        }
    }
}
