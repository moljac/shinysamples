using SQLite;
using System;


namespace Samples.Models
{
    public class SyncAttempt
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int BatchSize { get; set; }
        public bool IsProcessed { get; set; }
    }
}
