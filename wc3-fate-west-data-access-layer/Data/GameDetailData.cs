using System.Collections.Generic;

namespace wc3_fate_west_data_access_layer.Data
{
    public class GameDetailData
    {
        public int GameID { get; set; }
        public List<GamePlayerDetailData> Team1Data { get; set; }
        public List<GamePlayerDetailData> Team2Data { get; set; }
        public int Team1WinCount { get; set; }
        public int Team2WinCount { get; set; }
        public int Team1Kills { get; set; }
        public int Team1Deaths { get; set; }
        public int Team1Assists { get; set; }
        public int Team1Gold { get; set; }
        public double Team1DamageDealt { get; set; }
        public int Team2Kills { get; set; }
        public int Team2Deaths { get; set; }
        public int Team2Assists { get; set; }
        public int Team2Gold { get; set; }
        public double Team2DamageDealt { get; set; }
    }
}
