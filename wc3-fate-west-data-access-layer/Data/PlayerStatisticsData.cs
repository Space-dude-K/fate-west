namespace wc3_fate_west_data_access_layer.Data
{
    public class PlayerStatisticsData
    {
        public string PlayerName { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public double WinRate { get; set; }
        public double KDA { get; set; }
        public double AvgDamageDealt { get; set; }
        public double AvgDamageTaken { get; set; }
        public double AvgGoldSpent { get; set; }
    }
}
