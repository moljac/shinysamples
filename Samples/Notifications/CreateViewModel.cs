using System;
using System.Windows.Input;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Acr.UserDialogs.Forms;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny.Notifications;
using Shiny;


namespace Samples.Notifications
{
    public class CreateViewModel : ViewModel
    {
        readonly INotificationManager notificationManager;


        public CreateViewModel(INotificationManager notificationManager, IUserDialogs dialogs)
        {
            this.notificationManager = notificationManager;

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

            this.SendNow = ReactiveCommand.CreateFromTask(() => this.BuildAndSend(
                "Test Now",
                "This is a test of the sendnow stuff",
                null
            ));
            this.Send = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    await this.BuildAndSend(
                        this.NotificationTitle,
                        this.NotificationMessage,
                        this.ScheduledTime
                    );
                    this.NotificationTitle = String.Empty;
                    this.NotificationMessage = String.Empty;
                    this.Payload = String.Empty;
                    await dialogs.Alert("Notification Sent Successfully");
                },
                this.WhenAny(
                    x => x.NotificationTitle,
                    x => x.NotificationMessage,
                    x => x.ScheduledTime,
                    (title, msg, sch) =>
                        !title.GetValue().IsEmpty() &&
                        !msg.GetValue().IsEmpty() &&
                        sch.GetValue() > DateTime.Now
                )
            );
            this.PermissionCheck = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await notificationManager.RequestAccess();
                dialogs.Toast("Permission Check Result: " + result);
            });
        }


        async Task BuildAndSend(string title, string message, DateTime? scheduleDate)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                BadgeCount = this.BadgeCount,
                ScheduleDate = scheduleDate,
                Category = this.UseActions ? "Test" : null,
                Sound = this.GetSound()
            };
            if (!this.Payload.IsEmpty())
            {
                notification.Payload = new Dictionary<string, string> {
                    { nameof(this.Payload), this.Payload }
                };
            }
            if (!this.AndroidChannel.IsEmpty())
            {
                notification.Android.ChannelId = this.AndroidChannel;
                notification.Android.Channel = this.AndroidChannel;
            }
            if (this.UseAndroidHighPriority)
            {
                notification.Android.Priority = 9;
                notification.Android.NotificationImportance = AndroidNotificationImportance.Max;
            }
            notification.Android.Vibrate = this.UseAndroidVibrate;
            notification.Android.UseBigTextStyle = this.UseAndroidBigTextStyle;

            await notificationManager.Send(notification);
        }

        NotificationSound GetSound()
        {
            switch (this.SelectedSoundType)
            {
                case "Default"  : return NotificationSound.DefaultSystem;
                case "Priority" : return NotificationSound.DefaultPriority;
                default         : return NotificationSound.None;
            }
        }

        public ICommand PermissionCheck { get; }
        public ICommand Send { get; }
        public ICommand SendNow { get; }

        [Reactive] public string NotificationTitle { get; set;} = "Test Title";
        [Reactive] public string NotificationMessage { get; set; } = "Test Message";
        public DateTime ScheduledTime { [ObservableAsProperty] get; }
        [Reactive] public bool UseActions { get; set; } = true;
        [Reactive] public DateTime SelectedDate { get; set; }
        [Reactive] public TimeSpan SelectedTime { get; set; }
        [Reactive] public int BadgeCount { get; set; }
        [Reactive] public string Payload { get; set; }
        [Reactive] public string AndroidChannel { get; set; }
        [Reactive] public bool UseAndroidVibrate { get; set; }
        [Reactive] public bool UseAndroidBigTextStyle { get; set; }
        [Reactive] public bool UseAndroidHighPriority { get; set; }
        public bool IsAndroid => Device.RuntimePlatform == Device.Android;

        public string[] SoundTypes { get; } = new[]
        {
            "None",
            "Default",
            "Priority"
        };
        [Reactive] public string SelectedSoundType { get; set; } = "None";
    }
}