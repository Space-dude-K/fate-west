using System;
using System.Collections.Generic;
using wc3_fate_west_data_access_layer.Data;

namespace wc3_fate_west_web.Models
{
    public class MainPageViewModel
    {
        public IEnumerable<GameData> RecentGameDataList { get; set; }
        public DateTime CurrentBotTime { get; set; }

    }
}