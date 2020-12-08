using System;
using Shiny;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Xamarin.Forms.Platform.Android;

[assembly: ShinyApplication(ShinyStartupTypeName = "Samples.SampleStartup")]

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
        //protected override void OnCreate(Bundle savedInstanceState)
        //{
        //    TabLayoutResource = Resource.Layout.Tabbar;
        //    ToolbarResource = Resource.Layout.Toolbar;

        //    base.OnCreate(savedInstanceState);
        //    Forms.SetFlags(
        //        "SwipeView_Experimental",
        //        "Expander_Experimental",
        //        "RadioButton_Experimental"
        //    );
        //    Forms.Init(this, savedInstanceState);

        //    XF.Material.Droid.Material.Init(this, savedInstanceState);
        //    this.LoadApplication(new App());

        //    this.ShinyOnCreate();
        //}
    }
}