using System;
using Shiny;
using Acr.UserDialogs;
using Prism;
using Prism.Ioc;
using Prism.Mvvm;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Samples.Infrastructure;
using Prism.DryIoc;
using DryIoc;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]


namespace Samples
{
    public partial class App : PrismApplication
    {
        protected override async void OnInitialized()
        {
            this.InitializeComponent();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType =>
            {
                var viewModelTypeName = viewType.FullName.Replace("Page", "ViewModel");
                var viewModelType = Type.GetType(viewModelTypeName);
                return viewModelType;
            });
            await this.NavigationService.Navigate("Main/Nav/Welcome");
        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
#if DEBUG
            Xamarin.Forms.Internals.Log.Listeners.Add(new TraceLogListener());
#endif
            containerRegistry.RegisterForNavigation<NavigationPage>("Nav");
            containerRegistry.RegisterForNavigation<MainPage>("Main");
            containerRegistry.RegisterForNavigation<WelcomePage>("Welcome");
            containerRegistry.RegisterForNavigation<Gps.MainPage>("Gps");

            containerRegistry.RegisterForNavigation<Beacons.MainPage>("Beacons");
            containerRegistry.RegisterForNavigation<Beacons.CreatePage>("CreateBeacon");
            containerRegistry.RegisterForNavigation<BluetoothLE.AdapterPage>("BleCentral");
            containerRegistry.RegisterForNavigation<BluetoothLE.PeripheralPage>("Peripheral");
            containerRegistry.RegisterForNavigation<BlePeripherals.MainPage>("BlePeripherals");

            containerRegistry.RegisterForNavigation<Geofences.MainPage>("Geofencing");
            containerRegistry.RegisterForNavigation<Geofences.CreatePage>("CreateGeofence");

            containerRegistry.RegisterForNavigation<HttpTransfers.MainPage>("HttpTransfers");
            containerRegistry.RegisterForNavigation<HttpTransfers.CreatePage>("CreateTransfer");
            containerRegistry.RegisterForNavigation<HttpTransfers.ManageUploadsPage>("ManageUploads");

            containerRegistry.RegisterForNavigation<Jobs.MainPage>("Jobs");
            containerRegistry.RegisterForNavigation<Jobs.CreatePage>("CreateJob");

            containerRegistry.RegisterForNavigation<Notifications.MainPage>("Notifications");
            containerRegistry.RegisterForNavigation<Speech.MainPage>("SpeechRecognition");

            containerRegistry.RegisterForNavigation<Sensors.MainPage>("Sensors");
            containerRegistry.RegisterForNavigation<Sensors.CompassPage>("Compass");

            containerRegistry.RegisterForNavigation<IO.MainPage>("IO");

            containerRegistry.RegisterForNavigation<Logging.LoggingPage>("Logs");
            containerRegistry.RegisterForNavigation<AccessPage>("Access");
            containerRegistry.RegisterForNavigation<EnvironmentPage>("Environment");
            containerRegistry.RegisterForNavigation<Settings.MainPage>("Settings");
        }



        protected override IContainerExtension CreateContainerExtension()
        {
            var container = new Container(Rules
                .Default
                .WithAutoConcreteTypeResolution()
                .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
                .WithoutThrowOnRegisteringDisposableTransient()
                .WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.Replace)
            );
            ShinyHost.Populate((serviceType, func, lifetime) =>
                container.RegisterDelegate(
                    serviceType,
                    _ => func(),
                    Reuse.Singleton // HACK: I know everything is singleton
                )
            );
            return new DryIocContainerExtension(container);
        //    var builder = new ContainerBuilder();
        //    ShinyHost.Populate((serviceType, func, lifetime) =>
        //        builder
        //            .Register(_ => func())
        //            .As(serviceType)
        //            .SingleInstance()
        //    );
        //    builder
        //        .RegisterType<GlobalExceptionHandler>()
        //        .As<IStartable>()
        //        .AutoActivate()
        //        .SingleInstance();

        //    return new AutofacContainerExtension(builder);
        }
    }
}
