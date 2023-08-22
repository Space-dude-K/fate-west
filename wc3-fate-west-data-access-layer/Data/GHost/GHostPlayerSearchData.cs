using System;

namespace wc3_fate_west_data_access_layer.Data.GHost
{
    public class GHostPlayerSearchData
    {
        public string PlayerName { get; set; }
        public string Ip { get; set; }
        public string GameName { get; set; }
        public DateTime DateTime { get; set; }
        public int Duration { get; set; }
        public string LeftReason { get; set; }
        public string SpoofedRealm { get; set; }
    }
}
