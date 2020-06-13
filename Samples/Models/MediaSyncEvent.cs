using SQLite;
using System;


namespace Samples.Models
{
    public class MediaSyncEvent
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string FilePath { get; set; }
        public string Type { get; set; }
        //public DateTime DateAssetCreated { get; set; }
        public DateTime DateCompleted { get; set; }
    }
}
