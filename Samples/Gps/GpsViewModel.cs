using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Shiny.Locations;
using Acr.UserDialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;

namespace Samples.Gps
{
    public class GpsViewModel : ViewModel
    {
        readonly IGpsManager manager;
        IDisposable gpsListener;


        public GpsViewModel(IGpsManager manager, IUserDialogs dialogs)
        {
            this.manager = manager;
            this.IsUpdating = this.manager.IsListening;
            this.Access = this.manager.Status.ToString();

            this.listenerText = this
                .WhenAnyValue(x => x.IsUpdating)
                .Select(x => x ? "Stop Listening" : "Start Updating")
                .ToProperty(this, x => x.ListenerText);

            this.GetCurrentPosition = ReactiveCommand.CreateFromTask(async _ =>
            {
                var result = await dialogs.RequestAccess(() => this.manager.RequestAccess(true));
                if (!result)
                    return;

                var reading = await this.manager.GetLastReading();
                if (reading == null)
                    dialogs.Alert("Could not getting GPS coordinates");
                else
                    this.SetValues(reading);
            });
            this.BindBusyCommand(this.GetCurrentPosition);

            this.ToggleUpdates = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    if (this.manager.IsListening)
                    {
                        await this.manager.StopListener();
                        this.gpsListener?.Dispose();
                    }
                    else
                    {
                        var result = await dialogs.RequestAccess(() => this.manager.RequestAccess(true));
                        if (!result)
                            return;

                        var request = new GpsRequest
                        {
                            UseBackground = true
                        };
                        var meters = ToDeferred(this.DeferredMeters);
                        if (meters > 0)
                            request.DeferredDistance = Distance.FromMeters(meters);

                        var secs = ToDeferred(this.DeferredSeconds);
                        if (secs > 0)
                            request.DeferredTime = TimeSpan.FromSeconds(secs);

                        await this.manager.StartListener(request);
                    }
                    this.IsUpdating = this.manager.IsListening;
                },
                this.WhenAny(
                    x => x.IsUpdating,
                    x => x.DeferredMeters,
                    x => x.DeferredSeconds,
                    (u, m, s) =>
                        u.GetValue() ||
                        (
                            ToDeferred(m.GetValue()) >= 0 &&
                            ToDeferred(s.GetValue()) >= 0
                        )
                )
            );

            this.RequestAccess = ReactiveCommand.CreateFromTask(async () =>
            {
                var access = await this.manager.RequestAccess(true);
                this.Access = access.ToString();
            });
            this.BindBusyCommand(this.RequestAccess);
        }


        public override void OnAppearing()
        {
            base.OnAppearing();

            this.gpsListener = this.manager
                .WhenReading()
                .SubOnMainThread(this.SetValues)
                .DisposeWith(this.DeactivateWith);
        }


        void SetValues(IGpsReading reading)
        {
            using (this.DelayChangeNotifications())
            {
                this.Latitude = reading.Position.Latitude;
                this.Longitude = reading.Position.Longitude;
                this.Altitutde = reading.Altitude;
                this.PositionAccuracy = reading.PositionAccuracy;

                this.Heading = reading.Heading;
                this.HeadingAccuracy = reading.HeadingAccuracy;
                this.Speed = reading.Speed;
            }
        }


        public ReactiveCommand<Unit, Unit> GetCurrentPosition { get; }
        public ReactiveCommand<Unit, Unit> ToggleUpdates { get; }
        public ReactiveCommand<Unit, Unit> RequestAccess { get; }


        readonly ObservableAsPropertyHelper<string> listenerText;
        public string ListenerText => this.listenerText.Value;

        [Reactive] public string DeferredMeters { get; set; }
        [Reactive] public string DeferredSeconds { get; set; }
        [Reactive] public string Access { get; private set; }
        [Reactive] public bool IsUpdating { get; private set; }
        [Reactive] public double Latitude { get; private set; }
        [Reactive] public double Longitude { get; private set; }
        [Reactive] public double Altitutde { get; private set; }
        [Reactive] public double PositionAccuracy { get; private set; }
        [Reactive] public double Heading { get; private set; }
        [Reactive] public double HeadingAccuracy { get; private set; }
        [Reactive] public double Speed { get; private set; }


        static int ToDeferred(string value)
        {
            if (value.IsEmpty())
                return 0;

            if (Int32.TryParse(value, out int r))
                return r;

            return -1;
        }
    }
}
