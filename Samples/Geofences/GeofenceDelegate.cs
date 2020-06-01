using System;
using System.Threading.Tasks;
using Samples.Infrastructure;
using Samples.Models;
using Shiny.Locations;


namespace Samples.Geofences
{
    public class GeofenceDelegate : IGeofenceDelegate
    {
        readonly CoreDelegateServices services;
        public GeofenceDelegate(CoreDelegateServices services) => this.services = services;


        public async Task OnStatusChanged(GeofenceState newStatus, GeofenceRegion region)
        {
            await this.services.Connection.InsertAsync(new GeofenceEvent
            {
                Identifier = region.Identifier,
                Entered = newStatus == GeofenceState.Entered,
                Date = DateTime.Now
            });
            var notify = newStatus == GeofenceState.Entered
                ? this.services.AppSettings.UseNotificationsGeofenceEntry
                : this.services.AppSettings.UseNotificationsGeofenceExit;

            await this.services.SendNotification(
                "Geofence Event",
                $"{region.Identifier} was {newStatus}",
                x => x.UseNotificationsBle
            );
        }
    }
}
