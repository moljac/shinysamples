using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;
using Shiny.MediaSync;


namespace Samples.MediaSync
{
    public class SettingsViewModel : ViewModel
    {
        public SettingsViewModel(SampleMediaSyncDelegate? syncDelegate = null, IMediaSyncManager? manager = null)
        {
            this.IsEnabled = manager != null;
            manager?.ReflectCopyTo(this);
            syncDelegate?.ReflectCopyTo(this);

            this.WhenAnyValue(
                    x => x.CanSyncPhotos,
                    x => x.CanSyncVideos,
                    x => x.CanSyncAudio
                )
                .Skip(1)
                .Subscribe(_ => 
                { 
                    syncDelegate.CanSyncAudio = this.CanSyncAudio;
                    syncDelegate.CanSyncImages = this.CanSyncPhotos;
                    syncDelegate.CanSyncVideos = this.CanSyncVideos;
                })
                .DisposeWith(this.DestroyWith);

            this.RequestPermission = ReactiveCommand.CreateFromTask(async () =>
            {
                if (manager == null)
                    this.Permission = "Not Supported";
                else
                {
                    var result = await manager.RequestAccess();
                    this.Permission = result.ToString();
                }
            });

            this.WhenAnyValue(
                    x => x.IsVideoSyncEnabled,
                    x => x.IsPhotoSyncEnabled,
                    x => x.IsAudioSyncEnabled
                )
                .Skip(1)
                .Subscribe(_ => 
                { 
                    var syncTypes = MediaTypes.None;
                    if (this.IsAudioSyncEnabled)
                        syncTypes |= MediaTypes.Audio;

                    if (this.IsPhotoSyncEnabled)
                        syncTypes |= MediaTypes.Image;

                    if (this.IsVideoSyncEnabled)
                        syncTypes |= MediaTypes.Video;

                    manager.SyncTypes = syncTypes;
                })
                .DisposeWith(this.DestroyWith);

            this.WhenAnyValue(x => x.DefaultUploadUri)
                .Skip(1)
                .Subscribe(x => manager.DefaultUploadUri = x)
                .DisposeWith(this.DestroyWith);


            this.WhenAnyValue(x => x.DefaultUploadUri)
                .Skip(1)
                .Subscribe(x => manager.DefaultUploadUri = x)
                .DisposeWith(this.DestroyWith);
        }


        public ICommand RequestPermission { get; }
        public bool IsEnabled { get; }
        [Reactive] public string DefaultUploadUri { get; set; }
        [Reactive] public bool ShowBadgeCount { get; set; }
        [Reactive] public bool IsVideoSyncEnabled { get; set; }
        [Reactive] public bool IsPhotoSyncEnabled { get; set; }
        [Reactive] public bool IsAudioSyncEnabled { get; set; }
        [Reactive] public bool CanSyncPhotos { get; set; }
        [Reactive] public bool CanSyncAudio { get; set; }
        [Reactive] public bool CanSyncVideos { get; set; }
        [Reactive] public bool AllowUploadOnMeteredConnection { get; set; }
        [Reactive] public DateTimeOffset SyncFrom { get; set; }
        [Reactive] public string Permission { get; private set; } = "Unknown";


        public override void OnAppearing()
        {
            base.OnAppearing();
            this.RequestPermission.Execute(null);
        }
    }
}
