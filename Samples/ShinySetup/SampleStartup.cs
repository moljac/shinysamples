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
using Acr.UserDialogs.Forms;
//using Prism.DryIoc;


namespace Samples.ShinySetup
{
    public class SampleStartup : ShinyStartup
    {
        public override void ConfigureServices(IServiceCollection builder)
        {
            builder.UseAppCenterLogging(Constants.AppCenterTokens, true, false);
            Log.AddLogger(new DbLogger());
#if DEBUG
            Log.UseConsole();
            Log.UseDebug();
#endif

            builder.AddSingleton<IFullService, FullService>();

            // create your infrastructures
            // jobs, connectivity, power, filesystem, are installed automatically
            builder.AddSingleton<SampleSqliteConnection>();
            builder.AddSingleton<CoreDelegateServices>();
            builder.RegisterStartupTask<GlobalExceptionHandler>();
            builder.AddSingleton<IUserDialogs, UserDialogs>();
            //builder.UseXamarinFormsDependencyService();

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

            //builder.UseGeofencing<LocationDelegates>(new GeofenceRegion("Test", new Position(1, 1), Distance.FromKilometers(1)));
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


        //public override IServiceProvider CreateServiceProvider(IServiceCollection services)
        //    => PrismContainerExtension.Current.CreateServiceProvider(services);
    }
}
