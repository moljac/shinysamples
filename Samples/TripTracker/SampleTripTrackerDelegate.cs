using Shiny;
using Shiny.Notifications;
using Shiny.TripTracker;
using System.Threading.Tasks;


namespace Samples.TripTracker
{
    public class SampleTripTrackerDelegate : ITripTrackerDelegate
    {
        const string N_TITLE = "Shiny Trip";
        readonly INotificationManager notifications;


        public SampleTripTrackerDelegate(INotificationManager notifications)
        {
            this.notifications = notifications;
        }


        public Task OnTripStart(Trip trip) => this.notifications.Send(N_TITLE, "Starting a new trip");


        public async Task OnTripEnd(Trip trip)
        {
            this.notifications.Badge = 1;

            var km = Distance.FromMeters(trip.TotalDistanceMeters).TotalKilometers;
            var avgSpeed = Distance.FromMeters(trip.AverageSpeedMetersPerHour).TotalKilometers;
            var time = trip.DateFinished - trip.DateStarted;

            await this.notifications.Send(N_TITLE, $"You just finished a trip that was {km} km and took {time.Value.TotalMinutes} minutes with an average speed of {avgSpeed} km");
        }
    }
}
