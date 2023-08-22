namespace wc3_fate_west_data_access_layer.Data
{
    public class ServantStatisticsData
    {
        public string HeroTypeId { get; set; }
        public string HeroUnitTypeID { get; set; }
        public string HeroName { get; set; }
        public string HeroTitle { get; set; }
        public int WinCount { get; set; }
        public int LossCount { get; set; }
        public double AvgKills { get; set; }
        public double AvgDeaths { get; set; }
        public double AvgAssists { get; set; }
        public double AvgGoldSpent { get; set; }
        public double AvgDamageDealt { get; set; }
        public double AvgDamageTaken { get; set; }
    }
}
