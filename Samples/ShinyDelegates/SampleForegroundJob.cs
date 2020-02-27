using Shiny.Jobs;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace Samples.ShinyDelegates
{
    public class SampleForegroundJob : IJob
    {
        public Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            throw new NotImplementedException();
        }
    }
}
