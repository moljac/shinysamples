using SQLite;
using System;


namespace Samples.Models
{
    public class NfcEvent
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
