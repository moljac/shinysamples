using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Infrastructure;
using Samples.Models;
using Samples.ShinyDelegates;
using Shiny.Locations.Sync;


namespace Samples.LocationSync
{
    public class ActionsViewModel : ViewModel
    {
        readonly SampleSqliteConnection conn;
        readonly LocationSyncDelegates syncDelegate;
        readonly ILocationSyncManager syncManager;
        readonly IDialogs dialogs;


        public ActionsViewModel(SampleSqliteConnection conn, 
                                LocationSyncDelegates syncDelegate, 
                                ILocationSyncManager syncManager,
                                IDialogs dialogs)
        {
            this.conn = conn;
            this.syncDelegate = syncDelegate;
            this.syncManager = syncManager;
            this.dialogs = dialogs;

            this.ProcessGeofences = this.ProcessCommand(LocationSyncType.Geofence);
            this.ClearGeofences = this.ClearCommand(LocationSyncType.Geofence);

            this.ProcessGps = this.ProcessCommand(LocationSyncType.GPS);
            this.ClearGps = this.ClearCommand(LocationSyncType.GPS);
        }


        public ICommand ClearGeofences { get; }
        public ICommand ProcessGeofences { get; }
        public ICommand ClearGps { get; }
        public ICommand ProcessGps { get; }
        [Reactive] public bool CanProcessSyncData { get; set; }


        public override void OnAppearing()
        {
            base.OnAppearing();
            this.CanProcessSyncData = this.syncDelegate.CanProcess;
            this.WhenAnyValue(x => x.CanProcessSyncData)
                .Skip(1)
                .Subscribe(x => this.syncDelegate.CanProcess = x)
                .DisposeWith(this.DeactivateWith);
        }


        ICommand ProcessCommand(LocationSyncType syncType) => ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await this.dialogs.Confirm("Run the sync?");
            if (!result)
                return;

            await this.dialogs.LoadingTask(() => this.syncManager.ForceRun(syncType), "Running Sync Processes");
            await this.dialogs.Snackbar("Sync Process Complete");
        });


        ICommand ClearCommand(LocationSyncType syncType) => ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await this.dialogs.Confirm("Are you sure you want to delete these events?");
            if (!result)
                return;

            await conn.DeleteAllAsync<LocationSyncEvent>();
            await syncManager.ClearEvents(syncType);
            await this.dialogs.Snackbar("Events Cleared");
        });
    }
}
