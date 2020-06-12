using Shiny.PhotoSync;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Samples.PhotoSync
{
    public class PhotoSyncDelegate : IPhotoSyncDelegate
    {
        public Task<bool> CanSync(Photo photo) => Task.FromResult(true);
        public Task<IDictionary<string, string>> GetUploadHeaders(Photo photo) => Task.FromResult<IDictionary<string, string>>(null);
        public Task OnPhotoSync(Photo photo) => Task.CompletedTask;
    }
}
