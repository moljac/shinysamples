using System;
using Foundation;
using UIKit;
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Acr.UserDialogs.Forms;
using Samples.ShinySetup;
using Shiny;
using UserNotifications;

namespace Samples.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate, IUNUserNotificationCenterDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // this needs to be loaded before EVERYTHING
            iOSShinyHost.Init(new SampleStartup());
            UNUserNotificationCenter.Current.Delegate = this;
            //iOSShinyHost.Init(ShinyStartup.FromAssemblyRegistration(typeof(App).Assembly));
            //iOSShinyHost.Init(ShinyStartup.AutoRegister());
            Forms.SetFlags("SwipeView_Experimental");

            Forms.Init();
            FormsMaps.Init();
            Rg.Plugins.Popup.Popup.Init();
            this.LoadApplication(new App());
            return base.FinishedLaunching(app, options);
        }


        //[Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        //public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        //{
        //    Console.WriteLine("RUNNING");
        //}

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
            => iOSShinyHost.RegisteredForRemoteNotifications(deviceToken);

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
            => iOSShinyHost.FailedToRegisterForRemoteNotifications(error);

        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
            => iOSShinyHost.PerformFetch(completionHandler);

        public override void HandleEventsForBackgroundUrl(UIApplication application, string sessionIdentifier, Action completionHandler)
            => iOSShinyHost.HandleEventsForBackgroundUrl(sessionIdentifier, completionHandler);

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
            => ShinyHost.Resolve<IUserDialogs>().Alert(notification.AlertBody, "Notification - " + notification.AlertTitle);
    }
}
