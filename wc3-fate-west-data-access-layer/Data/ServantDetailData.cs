namespace wc3_fate_west_data_access_layer.Data
{
    public class ServantDetailData
    {
        public string HeroUnitTypeID { get; set; }
        public string HeroTitle { get; set; }
        public string MapVersion { get; set; }
        public int PlayCount { get; set; }
        public int WinCount { get; set; }
        public int KillCount { get; set; }
        public int DeathCount { get; set; }
        public int AssistCount { get; set; }
        public double GoldSpent { get; set; }
        public double DamageDealt { get; set; }
        public double DamageTaken { get; set; }
        public int GameDuration { get; set; }
        public double WinScoreDifference { get; set; }
        public double LossScoreDifference { get; set; }
    }

    public class ServantDetailStatData
    {
        public string MapVersion { get; set; }
        public string StatTypeId { get; set; }
        public string StatTypeName { get; set; }
        public int Points { get; set; }
    }
}
