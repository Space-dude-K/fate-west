using System;
using System.Collections.Generic;

namespace wc3_fate_west_data_access_layer.Data
{
    public class PlayerGameSummaryData
    {
        public readonly List<PlayerGameTeamPlayerData> TeamList = new List<PlayerGameTeamPlayerData>();
        public int GameID { get; set; }
        public DateTime PlayedDate { get; set; }
        public string Team { get; set; }
        public int TeamOneWinCount { get; set; }
        public int TeamTwoWinCount { get; set; }
        public string GameResult { get; set; }
        public string HeroUnitTypeID { get; set; }
        public int HeroKills { get; set; }
        public int HeroDeaths { get; set; }
        public int HeroAssists { get; set; }
        public int HeroLevel { get; set; }
        public int GoldSpent { get; set; }
        public double DamageDealt { get; set; }
        public double DamageTaken { get; set; } 
    }
}
