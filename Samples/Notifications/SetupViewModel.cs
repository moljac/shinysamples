using System;
using System.Windows.Input;
using System.Reactive.Linq;
using Acr.UserDialogs.Forms;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny.Notifications;


namespace Samples.Notifications
{
    public class SetupViewModel : ViewModel
    {
        public SetupViewModel(INotificationManager notificationManager, IUserDialogs dialogs)
        {
            this.WhenAnyValue
            (
                x => x.SelectedDate,
                x => x.SelectedTime
            )
            .Select(x => new DateTime(
                x.Item1.Year,
                x.Item1.Month,
                x.Item1.Day,
                x.Item2.Hours,
                x.Item2.Minutes,
                x.Item2.Seconds)
            )
            .ToPropertyEx(this, x => x.ScheduledTime);

            this.SelectedDate = DateTime.Now;
            this.SelectedTime = DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(10));

            this.SendScheduled = ReactiveCommand.CreateFromTask(
                () => notificationManager.Send(new Notification
                {
                    Title = "Scheduled",
                    Message = "This is a test scheduled notification",
                    Payload = "scheduled",
                    ScheduleDate = this.ScheduledTime
                }),
                this.WhenAny(
                    x => x.ScheduledTime,
                    x => x.GetValue() > DateTime.Now
                )
            );
            this.SendImmediate = ReactiveCommand.CreateFromTask(
                () => notificationManager.Send(new Notification
                {
                    Title = "Immediate",
                    Message = "This is a immediate test notification",
                    Payload = "immediate"
                })
            );
            this.PermissionCheck = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await notificationManager.RequestAccess();
                dialogs.Toast("Permission Check Result: " + result);
            });
        }


        public ICommand PermissionCheck { get; }
        public ICommand SendImmediate { get; }

        public ICommand SendScheduled { get; }
        public DateTime ScheduledTime { [ObservableAsProperty] get; }
        [Reactive] public DateTime SelectedDate { get; set; }
        [Reactive] public TimeSpan SelectedTime { get; set; }
    }
}