using System;
using System.Threading.Tasks;
using Shiny.Locations.Sync;


namespace Samples.ShinyDelegates
{
    public class LocationSyncDelegates : IGeofenceSyncDelegate, IGpsSyncDelegate
    {
        public Task Process(GpsEvent[] events)
        {
            throw new NotImplementedException();
        }

        public Task Process(GeofenceEvent geofence)
        {
            throw new NotImplementedException();
        }
    }
}
