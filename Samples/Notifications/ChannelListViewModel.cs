using System;
using System.Windows.Input;
using Prism.Navigation;
using Samples.Infrastructure;
using Shiny.Notifications;


namespace Samples.Notifications
{
    public class ChannelListViewModel : ViewModel
    {
        public ChannelListViewModel(INavigationService navigator, INotificationManager manager, IDialogs dialogs)
        {
            this.Create = navigator.NavigateCommand("ChannelCreate");
        }


        public ICommand Create { get; }
    }
}
