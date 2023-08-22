using System.Collections.Generic;

namespace wc3_fate_west_data_access_layer.Data
{
    public class GamePlayerDetailData
    {
        public int GameID { get; set; }
        public int TeamOneWinCount { get; set; }
        public int TeamTwoWinCount { get; set; }
        public string PlayerName { get; set; }
        public string HeroUnitTypeID { get; set; }
        public string HeroImageURL { get; set; }
        public List<string> GodsHelpImageURLList { get; set; }
        public int Level { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int GoldSpent { get; set; }
        public double DamageDealt { get; set; }
        public double DamageTaken { get; set; }
        public string GodsHelpAbilIDConcat { get; set; }
        public List<string> GodsHelpAbilIDList { get; set; } 
        public string Team { get; set; }
    }
}
