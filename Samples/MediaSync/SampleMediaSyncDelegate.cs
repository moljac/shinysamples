using System;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;
using Shiny.MediaSync;


namespace Samples.MediaSync
{
    public class SampleMediaSyncDelegate : MediaSyncDelegate
    {
        [Reactive] public bool CanSyncImages { get; set; }
        [Reactive] public bool CanSyncVideos { get; set; }
        [Reactive] public bool CanSyncAudio { get; set; }


        public override async Task<bool> CanSync(MediaAsset media)
        {
            return this.CanSyncImages;
        }


        public override Task OnSyncCompleted(MediaAsset media)
        {
            // TODO: store
            return base.OnSyncCompleted(media);
        }
    }
}
