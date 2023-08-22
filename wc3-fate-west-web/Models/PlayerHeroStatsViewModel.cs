namespace wc3_fate_west_web.Models
{
    public class PlayerHeroStatsViewModel
    {
        public string HeroImageURL { get; set; }
        public string HeroName { get; set; }
        public int HeroWins { get; set; }
        public int HeroLosses { get; set; }
        public string HeroWLPercent { get; set; }
        public string HeroAverageKills { get; set; }
        public string HeroAverageDeaths { get; set; }
        public string HeroAverageAssists { get; set; }
        public string HeroAverageKDA { get; set; }
        public string HeroKDAColor { get; set; }
    }
}