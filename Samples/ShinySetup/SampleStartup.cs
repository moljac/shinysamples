using System;
using Shiny;
using Shiny.Logging;
using Microsoft.Extensions.DependencyInjection;
using Samples.Settings;
using Samples.ShinyDelegates;
using Acr.UserDialogs.Forms;
//using Prism.DryIoc;


namespace Samples.ShinySetup
{
    public class SampleStartup : ShinyStartup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            //services.UseAppCenterLogging(Constants.AppCenterTokens, true, false);
            //services.UseSqliteLogging(true, true);
            //services.UseSqliteCache();
            //services.UseSqliteSettings();
            //services.UseSqliteStorage();
#if DEBUG
            Log.UseConsole();
            Log.UseDebug();
#endif

            // create your infrastructures
            // jobs, connectivity, power, filesystem, are installed automatically
            services.AddSingleton<SampleSqliteConnection>();
            services.AddSingleton<CoreDelegateServices>();
            services.RegisterStartupTask<GlobalExceptionHandler>();
            services.AddSingleton<IFullService, FullService>();
            services.AddSingleton<IUserDialogs, UserDialogs>();

            // startup tasks
            services.AddSingleton<IFullService, FullService>();
            services.RegisterStartupTask<JobLoggerTask>();

            // configuration
            services.RegisterSettings<IAppSettings, AppSettings>("AppSettings");

            // register all of the shiny stuff you want to use
            services.UseHttpTransfers<HttpTransferDelegate>();
            services.UseBeacons<BeaconDelegate>();

            services.RegisterBleAdapterState<BleDelegates>();
            services.RegisterBleStateRestore<BleDelegates>();
            services.UseBleCentral();
            services.UseBlePeripherals();

            //builder.UseGeofencing<LocationDelegates>(new GeofenceRegion("Test", new Position(1, 1), Distance.FromKilometers(1)));
            services.UseGeofencing<LocationDelegates>();
            services.UseGps<LocationDelegates>();
            services.UseNotifications();
            services.UseSpeechRecognition();

            services.UseAccelerometer();
            services.UseAmbientLightSensor();
            services.UseBarometer();
            services.UseCompass();
            services.UseMagnetometer();
            services.UsePedometer();
            services.UseProximitySensor();
            services.UseHeartRateMonitor();
            services.UseTemperature();
            services.UseHumidity();
        }


#if DEBUG
        public override IServiceProvider CreateServiceProvider(IServiceCollection services)
            => services.BuildServiceProvider(true);
#endif
        //public override IServiceProvider CreateServiceProvider(IServiceCollection services)
        //    => PrismContainerExtension.Current.CreateServiceProvider(services);
    }
}
