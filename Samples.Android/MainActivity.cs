using System;
using Shiny;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Runtime;


namespace Samples.Droid
{
    [Activity(
        Label = "Shiny",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation
    )]
    public partial class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Forms.SetFlags(
                "SwipeView_Experimental",
                "Expander_Experimental",
                "RadioButton_Experimental"
            );
            Forms.Init(this, savedInstanceState);

            XF.Material.Droid.Material.Init(this, savedInstanceState);
            this.LoadApplication(new App());

            this.ShinyOnCreate();
        }

#if !PRODUCTION
// these are generated in the main sample

    //These methods will automatically be created for you as long as you:
        // 1. Have Shiny.Core install in this project
        // 2. Don't require customization on them (meaning you haven't implemented them yourself)
        // 3. Your activity class is marked as partial

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            this.ShinyOnNewIntent(intent);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            this.ShinyRequestPermissionsResult(requestCode, permissions, grantResults);
        }
#endif
    }
}