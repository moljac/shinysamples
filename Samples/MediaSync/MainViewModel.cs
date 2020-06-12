using System;
using ReactiveUI.Fody.Helpers;
using Shiny.MediaSync;


namespace Samples.MediaSync
{
    public class MainViewModel : ViewModel
    {
        readonly IMediaSyncManager manager;
        readonly SampleMediaSyncDelegate syncDelegate;
        readonly SampleSqliteConnection conn;


        public MainViewModel(SampleMediaSyncDelegate syncDelegate,
                             SampleSqliteConnection conn,
                             IMediaSyncManager? manager = null)
        {
            this.manager = manager;
            this.syncDelegate = syncDelegate;
            this.conn = conn;
        }


        [Reactive] public string DefaultUploadUri { get; set; }
        [Reactive] public bool ShowBadgeCount { get; set; }
        [Reactive] public bool IsVideoSyncEnabled { get; set; }
        [Reactive] public bool IsPhotoSyncEnabled { get; set; }
        [Reactive] public bool IsAudioSyncEnabled { get; set; }
        [Reactive] public bool CanSyncPhotos { get; set; }
        [Reactive] public bool CanSyncAudio { get; set; }
        [Reactive] public bool CanSyncVideo { get; set; }
        [Reactive] public bool AllowUploadOnMeteredConnection { get; set; }
    }
}
