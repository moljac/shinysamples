using Shiny.Notifications;
using Shiny.TripTracker;
using System.Threading.Tasks;


namespace Samples.TripTracker
{
    public class SampleTripTrackerDelegate : ITripTrackerDelegate
    {
        readonly ITripTrackerManager manager;
        readonly INotificationManager notifications;


        public SampleTripTrackerDelegate(INotificationManager notifications, ITripTrackerManager manager)
        {
            this.notifications = notifications;
            this.manager = manager;
        }


        public async Task OnTripEnd(Trip trip)
        {
        }


        public async Task OnTripStart(Trip trip)
        {
        }
    }
}
