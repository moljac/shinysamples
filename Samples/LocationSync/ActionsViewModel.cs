using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Models;
using Samples.ShinyDelegates;
using Shiny.Locations.Sync;
using XF.Material.Forms.UI.Dialogs;


namespace Samples.LocationSync
{
    public class ActionsViewModel : ViewModel
    {
        readonly SampleSqliteConnection conn;
        readonly LocationSyncDelegates syncDelegate;
        readonly ILocationSyncManager syncManager;
        readonly IMaterialDialog dialogs;


        public ActionsViewModel(SampleSqliteConnection conn, 
                                LocationSyncDelegates syncDelegate, 
                                ILocationSyncManager syncManager,
                                IMaterialDialog dialogs)
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
            var result = await this.dialogs.ConfirmAsync("Run the sync?") ?? false;
            if (!result)
                return;

            using (await this.dialogs.LoadingDialogAsync("Running Sync Processes"))
                await syncManager.ForceRun(syncType);

            await this.dialogs.SnackbarAsync("Sync Process Complete");
        });


        ICommand ClearCommand(LocationSyncType syncType) => ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await this.dialogs.ConfirmAsync("Are you sure you want to delete these events?") ?? false;
            if (!result)
                return;

            await conn.DeleteAllAsync<LocationSyncEvent>();
            await syncManager.ClearEvents(syncType);
            await this.dialogs.SnackbarAsync("Events Cleared");
        });
    }
}
