using System;
using System.Collections.Generic;
using System.Windows.Input;
using Prism.Navigation;
using Samples.Infrastructure;
using Shiny.Notifications;


namespace Samples.Notifications
{
    public class ChannelListViewModel : ViewModel
    {
        readonly INotificationManager notifications;
        readonly IDialogs dialogs;


        public ChannelListViewModel(INavigationService navigator, INotificationManager notifications, IDialogs dialogs)
        {
            this.notifications = notifications;
            this.dialogs = dialogs;
            this.Create = navigator.NavigateCommand("ChannelCreate");
        }


        public ICommand Create { get; }
        public IList<CommandItem> Channels { get; }

        public override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
