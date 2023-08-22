using System;

namespace wc3_fate_west_data_access_layer.Data
{
    public class GameData
    {
        public int GameID { get; set; }
        public string GameName { get; set; }
        public DateTime PlayedDate { get; set; }
        public string ServerName { get; set; }
        public TimeSpan Duration { get; set; }
        public string Result { get; set; }
        public string MapVersion { get; set; }
        public string ReplayUrl { get; set; }
        public string MatchType { get; set; }
        public int PlayerCount { get; set; }
        public int TeamOneWinCount { get; set; }
        public int TeamTwoWinCount { get; set; }
    }
}
