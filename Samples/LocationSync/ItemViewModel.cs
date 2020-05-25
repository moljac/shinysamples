using Samples.Models;
using System;


namespace Samples.LocationSync
{
    public class ItemViewModel
    {
        readonly LocationSyncEvent location;
        public ItemViewModel(LocationSyncEvent e) => this.location = e;

        public string Type => this.location.GeofenceIdentifier == null ? "GPS" : "Geofence";
    }
}
