using System.Collections.Generic;

namespace wc3_fate_west_web.Models
{
    public class PlayerGameBuildViewModel
    {
        public string PlayerName { get; set; }
        public string HeroIconURL { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }
        public int Attack { get; set; }
        public int Armor { get; set; }
        public int HealthRegen { get; set; }
        public int ManaRegen { get; set; }
        public int MoveSpeed { get; set; }
        public int GoldRegen { get; set; }
        public int PrelatiMana { get; set; }
        public int FirstCS { get; set; }
        public int SecondCS { get; set; }
        public int ThirdCS { get; set; }
        public int FourthCS { get; set; }
        public int WardCount { get; set; }
        public int FamiliarCount { get; set; }
        public int SpiritLinkCount { get; set; }
        public List<PlayerGameBuildAttribute> AttributeList { get; set; }
    }

    public class PlayerGameBuildAttribute
    {
        public string AttributeImgUrl { get; set; }
        public string AttributeName { get; set; }
    }
}