using Shiny.DataSync;
using System;
using System.Threading.Tasks;


namespace Samples.DataSync
{
    public class SampleDataSyncDelegate : IDataSyncDelegate
    {
        public Task Push(SyncItem item)
        {
            return Task.CompletedTask;
        }
    }
}
