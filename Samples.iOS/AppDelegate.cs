using System;
using Foundation;
using UIKit;
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Acr.UserDialogs.Forms;
using Samples.ShinySetup;
using Shiny;


namespace Samples.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // this needs to be loaded before EVERYTHING
            this.ShinyFinishedLaunching(new SampleStartup());
            //iOSShinyHost.Init(ShinyStartup.FromAssemblyRegistration(typeof(App).Assembly));
            //iOSShinyHost.Init(ShinyStartup.AutoRegister());
            Forms.SetFlags("SwipeView_Experimental");

            Forms.Init();
            FormsMaps.Init();
            Rg.Plugins.Popup.Popup.Init();
            this.LoadApplication(new App());
            return base.FinishedLaunching(app, options);
        }


        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
            => this.ShinyDidReceiveRemoteNotification(userInfo, completionHandler);

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
            => this.ShinyRegisteredForRemoteNotifications(deviceToken);

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
            => this.ShinyFailedToRegisterForRemoteNotifications(error);

        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
            => this.ShinyPerformFetch(completionHandler);

        public override void HandleEventsForBackgroundUrl(UIApplication application, string sessionIdentifier, Action completionHandler)
            => this.ShinyHandleEventsForBackgroundUrl(sessionIdentifier, completionHandler);

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
            => ShinyHost.Resolve<IUserDialogs>().Alert(notification.AlertBody, "Notification - " + notification.AlertTitle);
    }
}
