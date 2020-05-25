using System;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Models;
using Shiny.Locations.Sync;

using GeofenceEvent = Shiny.Locations.Sync.GeofenceEvent;
using GpsEvent = Shiny.Locations.Sync.GpsEvent;

namespace Samples.ShinyDelegates
{
    public class LocationSyncDelegates : ReactiveObject, IGeofenceSyncDelegate, IGpsSyncDelegate
    {
        readonly SampleSqliteConnection conn;
        public LocationSyncDelegates(SampleSqliteConnection conn) => this.conn = conn;


        [Reactive] public bool CanProcess { get; set; }


        public Task Process(GpsEvent[] events)
        {
            if (!this.CanProcess)
                throw new ArgumentException("No processing events right now");

            throw new NotImplementedException();
        }


        public Task Process(GeofenceEvent geofence)
        {
            if (!this.CanProcess)
                throw new ArgumentException("No processing events right now");


            throw new NotImplementedException();
        }


        async Task DoProcess(string identifier, string geofenceIdentifier, bool isGeofence)
        {
            var e = await this.conn.GetAsync<LocationSyncEvent>(identifier);

        }
    }
}
