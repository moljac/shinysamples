using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Samples
{
    public class DelegateNotificationsViewModel : ViewModel
    {
        public DelegateNotificationsViewModel(AppNotifications notifications)
        {

            //this.Notifications = new List<DelegateNotificationItemViewModel>
            //{
            //    new DelegateNotificationItemViewModel(settings, nameof(BluetoothLE.BleClientDelegate), "BluetoothLE"),
            //    new DelegateNotificationItemViewModel(settings, nameof(HttpTransfers.HttpTransferDelegate), "HTTP Transfers"),
            //    new DelegateNotificationItemViewModel(settings, nameof(Beacons.BeaconDelegate) + "Entry", "Beacon Region Entry"),
            //    new DelegateNotificationItemViewModel(settings, nameof(Beacons.BeaconDelegate) + "Exit", "Beacon Region Exit"),
            //    new DelegateNotificationItemViewModel(settings, nameof(Geofences.GeofenceDelegate) + "Entry", "Geofence Entry"),
            //    new DelegateNotificationItemViewModel(settings, nameof(Geofences.GeofenceDelegate) + "Exit", "Geofence Exit"),
            //    new DelegateNotificationItemViewModel(settings, nameof(Jobs.JobLoggerTask) + "Start", "Job Started"),
            //    new DelegateNotificationItemViewModel(settings, nameof(Jobs.JobLoggerTask) + "Finished", "Job Finished")
            //};
            this.Notifications = notifications
                .GetRegistrations()
                .Select(x => new DelegateNotificationItemViewModel(notifications, x))
                .ToList();

            this.WhenAnyValue(x => x.ToggleAll)
                .Skip(1)
                .Subscribe(x => this.Notifications.ForEach(y =>
                {
                    y.IsEntryEnabled = x;
                    y.IsExitEnabled = x;
                }))
                .DisposeWith(this.DeactivateWith);
        }


        public List<DelegateNotificationItemViewModel> Notifications { get; }
        [Reactive] public bool ToggleAll { get; set; }
    }


    public class DelegateNotificationItemViewModel : ReactiveObject
    {
        public DelegateNotificationItemViewModel(AppNotifications notifications, NotificationRegistration reg)
        {
            this.Description = reg.Description;
            this.Text = reg.HasEntryExit ? "Entry" : "Enabled";
            this.HasEntryExit = reg.HasEntryExit;

            this.WhenAnyValue(x => x.IsEntryEnabled)
                .Skip(1)
                .Subscribe(x => notifications.Set(reg.Type, true, x));

            this.WhenAnyValue(x => x.IsExitEnabled)
                .Skip(1)
                .Subscribe(x => notifications.Set(reg.Type, false, x));
        }


        public string Description { get; }
        public string Text { get; }
        public bool HasEntryExit { get; }
        [Reactive] public bool IsEntryEnabled { get; set; }
        [Reactive] public bool IsExitEnabled { get; set; }
    }
}