using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Settings;
using Shiny;
using Shiny.Settings;


namespace Samples
{
    public class DelegateNotificationsViewModel : ViewModel
    {
        public DelegateNotificationsViewModel(ISettings settings, IAppSettings appSettings)
        {
            this.Notifications = new List<DelegateNotificationItemViewModel>
            {
                new DelegateNotificationItemViewModel(settings, nameof(BluetoothLE.BleClientDelegate), "BluetoothLE"),
                new DelegateNotificationItemViewModel(settings, nameof(HttpTransfers.HttpTransferDelegate), "HTTP Transfers"),
                new DelegateNotificationItemViewModel(settings, nameof(Beacons.BeaconDelegate) + "Entry", "Beacon Region Entry"),
                new DelegateNotificationItemViewModel(settings, nameof(Beacons.BeaconDelegate) + "Exit", "Beacon Region Exit"),
                new DelegateNotificationItemViewModel(settings, nameof(Geofences.GeofenceDelegate) + "Entry", "Geofence Entry"),
                new DelegateNotificationItemViewModel(settings, nameof(Geofences.GeofenceDelegate) + "Exit", "Geofence Exit"),
                new DelegateNotificationItemViewModel(settings, nameof(Jobs.JobLoggerTask) + "Start", "Job Started"),
                new DelegateNotificationItemViewModel(settings, nameof(Jobs.JobLoggerTask) + "Finished", "Job Finished")
            };

            this.WhenAnyValue(x => x.ToggleAll)
                .Skip(1)
                .Subscribe(x => this.Notifications.ForEach(y => y.IsEnabled = x))
                .DisposeWith(this.DeactivateWith);
        }


        public List<DelegateNotificationItemViewModel> Notifications { get; }
        [Reactive] public bool ToggleAll { get; set; }
    }


    public class DelegateNotificationItemViewModel : ReactiveObject
    {
        public DelegateNotificationItemViewModel(ISettings settings, string key, string description)
        {
            this.Key = key;
            this.Description = description;
            this.IsEnabled = settings.Get<bool>(key);

            this.WhenAnyValue(x => x.IsEnabled)
                .Skip(1)
                .Subscribe(x => settings.Set(Key, x));
        }


        public string Key { get; }
        public string Description { get; }
        public bool IsEnabled { get; set; }
    }
}