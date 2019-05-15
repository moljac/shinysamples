using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SQLite;
using Shiny;
using Shiny.Beacons;
using Shiny.BluetoothLE;
using Shiny.BluetoothLE.Central;
using Shiny.Locations;
using Shiny.Jobs;
using Shiny.Net.Http;
using Shiny.Notifications;
using Samples.Models;
using Samples.Settings;


namespace Samples.ShinySetup
{
    public class SampleAllDelegate : IGeofenceDelegate,
                                     IGpsDelegate,
                                     IBeaconDelegate,
                                     IHttpTransferDelegate,
                                     IBleStateRestoreDelegate,
                                     IBleAdapterDelegate,
                                     IJob
    {
        // notice you can inject anything you registered in your application here
        readonly SampleSqliteConnection conn;
        readonly INotificationManager notifications;
        readonly AppSettings appSettings;


        public SampleAllDelegate(SampleSqliteConnection conn,
                                 AppSettings appSettings,
                                 INotificationManager notifications)
        {
            this.conn = conn;
            this.appSettings = appSettings;
            this.notifications = notifications;
        }


        public async void OnBleAdapterStateChanged(AccessState state)
        {
            if (state != AccessState.Available && this.appSettings.UseNotificationsBle)
            {
                await this.notifications.Send(new Notification
                {
                    Title = "BLE State",
                    Message = "Turn on Bluetooth already"
                });
            }
        }


        public void OnConnected(IPeripheral peripheral)
        {
            //await this.DoNotification(
            //    "BluetoothLE Device Connected",
            //    $"{region.Identifier} was {newStatus}",
            //    this.appSettings.UseBleNotifications
            //);
        }


        public async void OnStatusChanged(GeofenceState newStatus, GeofenceRegion region)
        {
            await this.conn.InsertAsync(new GeofenceEvent
            {
                Identifier = region.Identifier,
                Entered = newStatus == GeofenceState.Entered,
                Date = DateTime.Now
            });
            var notify = newStatus == GeofenceState.Entered
                ? this.appSettings.UseNotificationsGeofenceEntry
                : this.appSettings.UseNotificationsGeofenceExit;

            await this.DoNotification(
                "Geofence Event",
                $"{region.Identifier} was {newStatus}",
                notify
            );
        }


        public async void OnStatusChanged(BeaconRegionState newStatus, BeaconRegion region)
        {
            await this.conn.InsertAsync(new BeaconEvent
            {
                Identifier = region.Identifier,
                Uuid = region.Uuid,
                Major = region.Major,
                Minor = region.Minor,
                Entered = newStatus == BeaconRegionState.Entered,
                Date = DateTime.UtcNow
            });
            var notify = newStatus == BeaconRegionState.Entered
                ? this.appSettings.UseNotificationsBeaconRegionEntry
                : this.appSettings.UseNotificationsBeaconRegionExit;

            await this.DoNotification
            (
                "Beacon Region {newStatus}",
                $"{region.Identifier} - {region.Uuid}/{region.Major}/{region.Minor}",
                notify
            );
        }


        public async Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            await this.DoNotification(
                "Job Started",
                $"{jobInfo.Identifier} Started",
                this.appSettings.UseNotificationsJobStart
            );
            var loops = jobInfo.Parameters.Get("Loops", 10);
            for (var i = 0; i < loops; i++)
            {
                if (cancelToken.IsCancellationRequested)
                    break;

                await Task.Delay(1000, cancelToken).ConfigureAwait(false);
            }
            await this.DoNotification(
                "Job Finished",
                $"{jobInfo.Identifier} Finished",
                this.appSettings.UseNotificationsJobFinish
            );

            // you really shouldn't lie about this on iOS as it is watching :)
            return true;
        }


        public async void OnReading(IGpsReading reading)
        {
            await this.conn.InsertAsync(new GpsEvent
            {
                Latitude = reading.Position.Latitude,
                Longitude = reading.Position.Longitude,
                Altitude = reading.Altitude,
                PositionAccuracy = reading.PositionAccuracy,
                Heading = reading.Heading,
                HeadingAccuracy = reading.HeadingAccuracy,
                Speed = reading.Speed,
                Date = reading.Timestamp.ToLocalTime()
            });
        }


        public async void OnError(HttpTransfer transfer, Exception ex)
            => await this.CreateHttpTransferEvent(transfer, "ERROR: " + ex);


        public async void OnCompleted(HttpTransfer transfer)
        {
            if (!transfer.IsUpload && Path.GetExtension(transfer.LocalFilePath) == "db")
            {
                try
                {
                    using (var conn = new SQLiteConnection(transfer.LocalFilePath))
                    {
                        var count = conn.Execute("SELECT COUNT(*) FROM sqlite_master WHERE type='table'");
                        await this.CreateHttpTransferEvent(transfer, $"COMPLETE - SQLITE PASSED ({count} tables)");
                    }
                }
                catch (Exception ex)
                {
                    await this.CreateHttpTransferEvent(transfer, $"COMPLETE - SQLITE FAILED - " + ex);
                }
            }
            else
            {
                await this.CreateHttpTransferEvent(transfer, "COMPLETE");
            }
        }


        async Task CreateHttpTransferEvent(HttpTransfer transfer, string description)
        {
            await this.conn.InsertAsync(new HttpEvent
            {
                Identifier = transfer.Identifier,
                IsUpload = transfer.IsUpload,
                FileSize = transfer.FileSize,
                Uri = transfer.Uri,
                Description = description,
                DateCreated = DateTime.Now
            });
            await this.DoNotification("HTTP Transfer", description, appSettings.UseNotificationsHttpTransfers);
        }


        async Task DoNotification(string title, string message, bool enabled)
        {
            if (enabled)
                await this.notifications.Send(new Notification { Title = title, Message = message });
        }
    }
}
