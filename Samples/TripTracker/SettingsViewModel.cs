using System;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Infrastructure;
using Shiny;
using Shiny.Locations;
using Shiny.TripTracker;


namespace Samples.TripTracker
{
    public class SettingsViewModel : ViewModel
    {
        public SettingsViewModel(ITripTrackerManager manager, IDialogs dialogs)
        {
            this.IsEnabled = manager.TrackingActivityType == null;
            this.UseAutomotive = manager.TrackingActivityType == MotionActivityType.Automotive;
            this.UseCycling = manager.TrackingActivityType == MotionActivityType.Cycling;
            this.UseRunning = manager.TrackingActivityType == MotionActivityType.Running;
            this.UseWalking = manager.TrackingActivityType == MotionActivityType.Walking;

            this.ToggleMonitoring = ReactiveCommand.CreateFromTask
            (
                async () =>
                {
                    var access = await manager.RequestAccess();
                    if (access != AccessState.Available)
                    {
                        await dialogs.Alert("Invalid Access - " + access);
                    }
                    else
                    {
                        if (!this.IsEnabled)
                        {
                            await manager.StopTracking();
                        }
                        else
                        {
                            var type = this.GetTrackingType().Value;
                            await manager.StartTracking(type);
                        }
                        this.IsEnabled = !this.IsEnabled;
                        this.RaisePropertyChanged(nameof(this.MonitoringText));
                    }
                },
                this.WhenAny(
                    x => x.UseAutomotive,
                    x => x.UseRunning,
                    x => x.UseWalking,
                    x => x.UseCycling,
                    (auto, run, walk, cycle) => this.GetTrackingType() != null
                )
            );
        }


        public ICommand ToggleMonitoring { get; }
        public string MonitoringText => this.IsEnabled ? "Start Monitoring" : "Stop Monitoring";
        [Reactive] public bool IsEnabled { get; private set; }
        [Reactive] public bool UseAutomotive { get; set; }
        [Reactive] public bool UseWalking { get; set; }
        [Reactive] public bool UseRunning { get; set; }
        [Reactive] public bool UseCycling { get; set; }


        MotionActivityType? GetTrackingType()
        {
            if (this.UseAutomotive)
                return MotionActivityType.Automotive;

            if (this.UseCycling)
                return MotionActivityType.Cycling;

            if (this.UseRunning)
                return MotionActivityType.Running;

            if (this.UseWalking)
                return MotionActivityType.Walking;

            return null;
        }
    }
}
