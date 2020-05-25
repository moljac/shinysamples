using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

using Prism.Services.Dialogs;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Models;
using Samples.ShinyDelegates;
using Shiny.Locations.Sync;
using XF.Material.Forms.UI.Dialogs;


namespace Samples.LocationSync
{
    public class MainViewModel : ViewModel
    {
        readonly SampleSqliteConnection conn;
        readonly LocationSyncDelegates syncDelegate;
        readonly ILocationSyncManager syncManager;


        public MainViewModel(SampleSqliteConnection conn,
                             IMaterialDialog dialog,
                             LocationSyncDelegates syncDelegate,
                             ILocationSyncManager syncManager)
        {
            this.conn = conn;
            this.syncDelegate = syncDelegate;
            this.syncManager = syncManager;

            this.Load = ReactiveCommand.CreateFromTask(async () =>
            {

            });
            this.BindBusyCommand(this.Load);

            this.Clear = ReactiveCommand.CreateFromTask(async () =>
            {
                await conn.DeleteAllAsync<LocationSyncEvent>();
                await syncManager.ClearGeofenceEvents();
                await syncManager.ClearGpsEvents();
                await dialog.SnackbarAsync("Location Sync Events Cleared");
            });

            this.ForceRun = ReactiveCommand.CreateFromTask(async () =>
            {
                using (await dialog.LoadingSnackbarAsync("Running Sync Processes"))
                    await syncManager.ForceRun();
            });
        }


        public ICommand Load { get; }
        public ICommand Clear { get; }
        public ICommand ForceRun { get; }
        [Reactive] public bool CanProcessSyncData { get; set; }
        [Reactive] public IList<ItemViewModel> Events { get; set; }


        public override void OnAppearing()
        {
            base.OnAppearing();
            this.Load.Execute(null);

            this.WhenAnyValue(x => x.CanProcessSyncData)
                .Skip(1)
                .Subscribe(x => this.syncDelegate.CanProcess = x)
                .DisposeWith(this.DeactivateWith);
        }
    }
}
