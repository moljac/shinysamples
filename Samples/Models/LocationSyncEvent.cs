using SQLite;

using System;


namespace Samples.Models
{
    public class LocationSyncEvent
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        public string GeofenceIdentifier { get; set; }
        public bool Pending { get; set; }
        public DateTime? DateSync { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
