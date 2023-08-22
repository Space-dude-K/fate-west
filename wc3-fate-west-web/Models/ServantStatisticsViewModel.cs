namespace wc3_fate_west_web.Models
{
    public class ServantStatisticsViewModel
    {
        public string HeroTypeId { get; set; }
        public string HeroImageURL { get; set; }
        public string HeroName { get; set; }
        public string HeroTitle { get; set; }
        public double WinRatioVal { get; set; }
        public string WinRatio { get; set; }
        public int PlayCount { get; set; }
        public string AvgKDA { get; set; }
        public string ProgressBarColor { get; set; }
        public string AvgGoldSpent { get; set; }
        public string AvgDamageDealt { get; set; }
        public string AvgDamageTaken { get; set; }
    }

    public class ServantStatisticsDetailViewModel
    {
        public string WinRate { get; set; }
        public string PickRate { get; set; }
        public string AvgKills { get; set; }
        public string AvgDeaths { get; set; }
        public string AvgAssists { get; set; }
        public string AvgKDA { get; set; }
        public int PlayCount { get; set; }
        public string MapVersion { get; set; }
        public string AvgGoldSpent { get; set; }
        public string AvgDamageDealt { get; set; }
        public string AvgDamageTaken { get; set; }
        public string AvgGameDuration { get; set; }
        public string AvgWinScoreDifference { get; set; }
        public string AvgLossScoreDifference { get; set; }
        public string AvgStr { get; set; }
        public string AvgAgi { get; set; }
        public string AvgInt { get; set; }
        public string AvgAtk { get; set; }
        public string AvgHealthRegen { get; set; }
        public string AvgManaRegen { get; set; }
        public string AvgMovespeed { get; set; }
        public string AvgGoldRegen { get; set; }
        public string AvgArmor { get; set; }
        public string AvgPrelati { get; set; }
    }
}