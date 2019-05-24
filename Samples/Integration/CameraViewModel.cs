using System;
using System.Windows.Input;
using Acr.UserDialogs.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using ReactiveUI;
using Shiny.Locations;

namespace Samples.Integration
{
    public class CameraViewModel : ViewModel
    {
        public CameraViewModel(IUserDialogs dialogs, IGpsManager gpsManager)
        {
            this.TakePicture = ReactiveCommand.CreateFromTask(async () =>
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await dialogs.Alert("No Camera", ":( No camera available.", "OK");
                    return;
                }

                var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

                if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
                {
                    var response = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera, Permission.Storage);
                    cameraStatus = response[Permission.Camera];
                    storageStatus = response[Permission.Storage];
                }

                if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
                {
                    await gpsManager.StartListener(new GpsRequest { UseBackground = true });
                    //var gpsManager = PrismContainerExtension.Current.Resolve<IGpsManager>();
                    //await gpsManager.StopListener();
                    var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg",
                        CompressionQuality = 90,
                        //DefaultCamera = CameraDevice.Rear,
                        //MaxWidthHeight = (int)Width,
                        SaveToAlbum = true,
                        SaveMetaData = true
                    });
                    //await gpsManager.StartListener(new GpsRequest { UseBackground = true });
                    await gpsManager.StopListener();
                }
                else
                {
                    await dialogs.Alert("Permissions Denied", "Unable to take photos.", "Ok");
                }
            });
        }


        public ICommand TakePicture { get; }
    }
}
