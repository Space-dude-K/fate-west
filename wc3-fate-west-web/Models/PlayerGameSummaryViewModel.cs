using System.Collections.Generic;
using wc3_fate_west_data_access_layer.Data;

namespace wc3_fate_west_web.Models
{
    public class PlayerGameSummaryViewModel
    {
        public readonly List<PlayerGameTeamPlayerData> Team1List = new List<PlayerGameTeamPlayerData>();
        public readonly List<PlayerGameTeamPlayerData> Team2List = new List<PlayerGameTeamPlayerData>();
        public int GameID { get; set; }
        public string PlayedDate { get; set; }
        public string HeroImageURL { get; set; }
        public string Team { get; set; }
        public int TeamOneWinCount { get; set; }
        public int TeamTwoWinCount { get; set; }
        public string GameResult { get; set; }
        public int HeroLevel { get; set; }
        public int HeroKills { get; set; }
        public int HeroDeaths { get; set; }
        public int HeroAssists { get; set; }
        public string DamageDealt { get; set; }
        public string DamageTaken { get; set; }
        public string HeroKDA { get; set; }
        public string HeroKDAColor { get; set; }
        public string GoldSpent { get; set; }
    }
}