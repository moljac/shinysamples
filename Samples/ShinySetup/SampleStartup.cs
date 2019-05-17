using System;
using Shiny;
using Shiny.Beacons;
using Shiny.BluetoothLE;
using Shiny.Logging;
using Shiny.Locations;
using Shiny.Notifications;
using Shiny.Sensors;
using Shiny.SpeechRecognition;
using Shiny.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Samples.Settings;
using Samples.Logging;
using Samples.ShinyDelegates;


namespace Samples.ShinySetup
{
    public class SampleStartup : Startup
    {
        public override void ConfigureServices(IServiceCollection builder)
        {
            Log.AddLogger(new DbLogger());
#if DEBUG
            Log.UseConsole();
            Log.UseDebug();
#else
            Log.AddLogger(new AppCenterLogger(), true, true);
#endif

            // create your infrastructures
            // jobs, connectivity, power, filesystem, are installed automatically
            builder.AddSingleton<SampleSqliteConnection>();
            builder.AddSingleton<CoreDelegateServices>();

            // startup tasks
            builder.RegisterStartupTask<StartupTask1>();
            builder.RegisterStartupTask<StartupTask2>();
            builder.RegisterStartupTask<JobLoggerTask>();

            // configuration
            builder.RegisterSettings<IAppSettings, AppSettings>("AppSettings");

            // register all of the shiny stuff you want to use
            builder.UseHttpTransfers<HttpTransferDelegate>();
            builder.UseBeacons<BeaconDelegate>();

            builder.RegisterBleAdapterState<BleDelegates>();
            builder.RegisterBleStateRestore<BleDelegates>();
            builder.UseBleCentral();
            builder.UseBlePeripherals();

            builder.UseGeofencing<LocationDelegates>();
            builder.UseGps<LocationDelegates>();
            builder.UseNotifications();
            builder.UseSpeechRecognition();

            builder.UseAccelerometer();
            builder.UseAmbientLightSensor();
            builder.UseBarometer();
            builder.UseCompass();
            builder.UseMagnetometer();
            builder.UsePedometer();
            builder.UseProximitySensor();
        }
    }
}
