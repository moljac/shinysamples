using System;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny.Locations;
using Shiny.TripTracker;


namespace Samples.TripTracker
{
    public class SettingsViewModel : ViewModel
    {
        public SettingsViewModel(ITripTrackerManager manager)
        {
            this.IsEnabled = manager.TrackingActivityTypes == null;
            this.UseAutomotive = manager.TrackingActivityTypes?.HasFlag(MotionActivityType.Automotive) ?? false;
            this.UseCycling = manager.TrackingActivityTypes?.HasFlag(MotionActivityType.Cycling) ?? false;
            this.UseRunning = manager.TrackingActivityTypes?.HasFlag(MotionActivityType.Running) ?? false;
            this.UseWalking = manager.TrackingActivityTypes?.HasFlag(MotionActivityType.Walking) ?? false;

            this.ToggleMonitoring = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    if (this.IsEnabled)
                    {
                        await manager.StopTracking();
                    }
                    else
                    {
                        var types = this.GetTypes();
                        await manager.StartTracking(types);
                    }
                    this.IsEnabled = !this.IsEnabled;
                    this.RaisePropertyChanged(nameof(this.MonitoringText));
                },
                this.WhenAny(
                    x => x.UseAutomotive,
                    x => x.UseRunning,
                    x => x.UseWalking,
                    x => x.UseCycling,
                    (auto, run, walk, cycle) => auto.GetValue() || run.GetValue() || walk.GetValue() || cycle.GetValue()
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


        MotionActivityType GetTypes()
        {
            MotionActivityType type = 0;
            if (this.UseAutomotive)
                type |= MotionActivityType.Automotive;

            if (this.UseCycling)
                type |= MotionActivityType.Cycling;

            if (this.UseRunning)
                type |= MotionActivityType.Running;

            if (this.UseWalking)
                type |= MotionActivityType.Walking;

            return type;
        }
    }
}
