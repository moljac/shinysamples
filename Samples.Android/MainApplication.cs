using System;
using Shiny;
using Shiny.Jobs;
using Android.App;
using Android.Runtime;
using Samples.ShinySetup;


namespace Samples.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    //public class MainApplication : ShinyAndroidApplication<SampleStartup>
    public class MainApplication : Application
    {
        public MainApplication() : base() { }
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }


        public override void OnCreate()
        {
            base.OnCreate();

            AndroidShinyHost.Init(
                this,
                new SampleStartup()
#if DEBUG
                , services =>
                {
                    // TODO: make android great again - by running jobs faster for debugging purposes ;)
                    services.ConfigureJobService(TimeSpan.FromMinutes(1));
                }
#endif
            );
        }
    }
}