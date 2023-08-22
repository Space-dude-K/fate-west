using System;

namespace wc3_fate_west_data_access_layer.Data.Wc3_Ghost
{
    public class Stats
    {
        private bool isReal;
        private string firstGameTime = "0";
        private string lastGameTime = "0";

        private string totalGames = "0";

        private string minLoadingTime = "0";
        private string avgLoadingTime = "0";
        private string maxLoadingTime = "0";

        private string minStayPercent = "0";
        private string avgStayPercent = "0";
        private string maxStayPercent = "0";

        private string minDurationTime = "0";
        private string avgDurationTime = "0";
        private string maxDurationTime = "0";

        public string FirstGameTime { get => firstGameTime; set => firstGameTime = value; }
        public string LastGameTime { get => lastGameTime; set => lastGameTime = value; }
        public string TotalGames { get => totalGames; set => totalGames = value; }
        public string MinLoadingTime { get => Convert.ToString(float.Parse(minLoadingTime) / 1000); set => minLoadingTime = value; }
        public string AvgLoadingTime { get => Convert.ToString(Math.Round(float.Parse(avgLoadingTime) / 1000, 1)); set => avgLoadingTime = value; }
        public string MaxLoadingTime { get => Convert.ToString(float.Parse(maxLoadingTime) / 1000); set => maxLoadingTime = value; }
        public string MinStayPercent { get => minStayPercent; set => minStayPercent = value; }
        public string AvgStayPercent { get => Convert.ToString(Math.Round(float.Parse(avgStayPercent), 1)); set => avgStayPercent = value; }
        public string MaxStayPercent { get => maxStayPercent; set => maxStayPercent = value; }
        public string MinDurationTime { get => minDurationTime; set => minDurationTime = value; }
        public string AvgDurationTime { get => avgDurationTime; set => avgDurationTime = value; }
        public string MaxDurationTime { get => maxDurationTime; set => maxDurationTime = value; }
        public bool IsReal { get => isReal; set => isReal = value; }

        public Stats(bool isReal)
        {
            this.isReal = isReal;
        }
    }
}