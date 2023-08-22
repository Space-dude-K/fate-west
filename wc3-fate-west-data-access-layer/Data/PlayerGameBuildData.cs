using System;
using System.Collections.Generic;

namespace wc3_fate_west_data_access_layer.Data
{
    public class PlayerGameBuildData
    {
        public Dictionary<string, int> StatBuildDic { get; set; } 
        public List<Tuple<string,string>> LearnedAttributeList { get; set; } 
        public Dictionary<string, int> PurchasedItemsDic { get; set; }
        public Dictionary<string, int> CommandSealDic { get; set; } 
        public string HeroUnitTypeId { get; set; }
    }
}
