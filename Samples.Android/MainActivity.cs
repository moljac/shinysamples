using System;
using Shiny;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Xamarin;
using Xamarin.Forms;


namespace Samples.Droid
{
    [Activity(
        Label = "Shiny",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation
    )]
    public class MainActivity : ShinyFormsActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Forms.SetFlags("SwipeView_Experimental");
            Forms.Init(this, savedInstanceState);
            FormsMaps.Init(this, savedInstanceState);
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            this.LoadApplication(new App());

            Shiny.Notifications.NotificationManager.TryProcessIntent(this.Intent);
        }


        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            Shiny.Notifications.NotificationManager.TryProcessIntent(intent);
        }
    }
}