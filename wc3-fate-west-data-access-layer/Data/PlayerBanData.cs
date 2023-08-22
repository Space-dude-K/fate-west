using System;
using System.Collections.Generic;

namespace wc3_fate_west_data_access_layer.Data
{
    public class PlayerBanData
    {
        public string PlayerName { get; set; }
        public List<string> IpAddresses { get; set; }
        public bool IsPermanentBan { get; set; }
        public DateTime BannedDateTime { get; set; }
        public int RemainingDuration { get; set; }
        public DateTime? BannedUntil { get; set; }
        public string Reason { get; set; }
        public string Admin { get; set; }
    }
}
