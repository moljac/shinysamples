using System;
using Samples.Models;
using Shiny;


namespace Samples.ShinyDelegates
{
    public class AppStateDelegate : IAppStateDelegate
    {
        readonly SampleSqliteConnection conn;
        public AppStateDelegate(SampleSqliteConnection conn)       
            => this.conn = conn;


        public void OnBackground() => this.Store("Background");
        public void OnForeground() => this.Store("Foreground");
        public void OnStart() => this.Store("Start");
        public void OnTerminate() => this.Store("Terminate");


        void Store(string eventName) => this.conn.GetConnection().Insert(new AppStateEvent
        {
            Event = eventName,
            Timestamp = DateTime.UtcNow
        });
    }
}

