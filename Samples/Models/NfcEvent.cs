using Shiny.Nfc;
using SQLite;
using System;


namespace Samples.Models
{
    public class NfcEvent
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public byte[] Identifier { get; set; }
        public byte[] Payload { get; set; }
        public NfcPayloadType PayloadType { get; set;}
        public string Uri { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
