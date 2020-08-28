#if !PRODUCTION
// this is generated in the main sample

// You don't need this if you install Shiny.Core in your android project
using System;
using Shiny;
using Android.App;
using Android.Runtime;


namespace Samples.Droid
{
    [Application]
    public class MainApplication : ShinyAndroidApplication<SampleStartup>
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }


        public override void OnCreate()
        {
            base.OnCreate();
            Xamarin.Essentials.Platform.Init(this);
        }
    }
}
#endif