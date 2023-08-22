using System;
using System.Collections.Generic;
using wc3_fate_west_data_access_layer.Data;

namespace wc3_fate_west_web.Models
{
    public class PlayerStatsPageViewModel
    {
        public bool HasFoundUser { get; set; }
        public string UserName { get; set; }
        public string Server { get; set; }
        public DateTime LastPlayedDateTime { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public string WLPercent { get; set; }
        public string AveragePlayerKills { get; set; }
        public string AveragePlayerDeaths { get; set; }
        public string AveragePlayerAssists { get; set; }
        public string AveragePlayerKDA { get; set; }
        public int LastGameID { get; set; }
        public List<PlayerHeroStatsViewModel> PlayerHeroStatSummaryData { get; set; }
        public List<PlayerGameSummaryViewModel> PlayerGameSummaryData { get; set; }
        public List<SearchableServantData> SearchableServantData { get; set; }
    }
}