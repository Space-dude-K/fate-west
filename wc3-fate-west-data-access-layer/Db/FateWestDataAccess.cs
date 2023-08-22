using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace wc3_fate_west_data_access_layer.Db
{
    public class FateWestDataAccess
    {
        public partial class attributeinfo
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_attributeinfo_1", Order = 0)]
            public Int64 AttributeInfoID { get; set; }

            [NotNull]
            public string AttributeAbilID { get; set; }

            [NotNull]
            public string AttributeName { get; set; }

        }

        public partial class attributelearn
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_attributelearn_1", Order = 0)]
            public Int64 AttributeLearnID { get; set; }

            [NotNull]
            public Int32 FK_AttributeInfoID { get; set; }

            [NotNull]
            public Int32 FK_GamePlayerDetailID { get; set; }

        }

        public partial class ban
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_ban_1", Order = 0)]
            public Int64 BanID { get; set; }

            [NotNull]
            public string PlayerName { get; set; }

            [NotNull]
            public string Reason { get; set; }

            [NotNull]
            public DateTime BannedDateTime { get; set; }

            [NotNull]
            public string Admin { get; set; }

            public DateTime? BannedUntil { get; set; }

            [NotNull]
            public Boolean IsPermanentBan { get; set; }

            [NotNull]
            public Boolean IsCurrentlyBanned { get; set; }

            [NotNull]
            public string IpAddresses { get; set; }

            public DateTime? ModifiedDateTime { get; set; }

            public string ModifiedByAdmin { get; set; }

        }

        public partial class commandsealuses
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_commandsealuses_1", Order = 0)]
            public Int64 CommandSealUseID { get; set; }

            [NotNull]
            public string CommandSealAbilID { get; set; }

            [NotNull]
            public Int32 UseCount { get; set; }

            [NotNull]
            public Int32 FK_GamePlayerDetailID { get; set; }

        }

        public partial class game
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_game_1", Order = 0)]
            public Int64 GameID { get; set; }

            [Indexed(Name = "IX_fk_Game_Server", Order = 0)]
            [NotNull]
            public Int32 FK_ServerID { get; set; }

            [NotNull]
            public string GameName { get; set; }

            [NotNull]
            public DateTime PlayedDate { get; set; }

            [NotNull]
            public int Duration { get; set; }

            [NotNull]
            public string Result { get; set; }

            [NotNull]
            public string MapVersion { get; set; }

            public string ReplayUrl { get; set; }

            public string MatchType { get; set; }

            public String Log { get; set; }

            [NotNull]
            public Int32 TeamOneWinCount { get; set; }

            [NotNull]
            public Int32 TeamTwoWinCount { get; set; }

        }

        public partial class gameitempurchase
        {
            [Key, AutoIncrement]
            public Int64 GameItemPurchaseID { get; set; }

            [Indexed(Name = "IX_fk_GameItemPurchase_ItemID", Order = 0)]
            [NotNull]
            public Int32 FK_ItemID { get; set; }

            [NotNull]
            public Int32 FK_GamePlayerDetailID { get; set; }

            [NotNull]
            public Int32 ItemPurchaseCount { get; set; }

        }

        public partial class gameplayerdetail
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_gameplayerdetail_1", Order = 0)]
            public Int64 GamePlayerDetailID { get; set; }

            [NotNull]
            public Int32 FK_GameID { get; set; }

            [NotNull]
            public Int32 FK_PlayerID { get; set; }

            [NotNull]
            public Int32 FK_ServerID { get; set; }

            [NotNull]
            public Int32 FK_HeroTypeID { get; set; }

            [NotNull]
            public Int32 Kills { get; set; }

            [NotNull]
            public Int32 Deaths { get; set; }

            [NotNull]
            public Int32 Assists { get; set; }

            [NotNull]
            public string Team { get; set; }

            [NotNull]
            public string Result { get; set; }

            [NotNull]
            public Int32 ScoreDiff { get; set; }

            [NotNull]
            public Int32 ELODiff { get; set; }

            [NotNull]
            public Int32 GoldSpent { get; set; }

            [NotNull]
            public Double DamageDealt { get; set; }

            [NotNull]
            public Double DamageTaken { get; set; }

            [NotNull]
            public Int32 HeroLevel { get; set; }

        }

        public partial class godshelpinfo
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_godshelpinfo_1", Order = 0)]
            public Int64 GodsHelpInfoID { get; set; }

            [NotNull]
            public string GodsHelpAbilID { get; set; }

            [NotNull]
            public string GodsHelpName { get; set; }

        }

        public partial class godshelpuse
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_godshelpuse_1", Order = 0)]
            public Int64 GodsHelpUseID { get; set; }

            [NotNull]
            public Int32 FK_GodsHelpInfoID { get; set; }

            [NotNull]
            public Int32 FK_GamePlayerDetailID { get; set; }

        }

        public partial class herostatinfo
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_herostatinfo_1", Order = 0)]
            public Int64 HeroStatInfoID { get; set; }

            [NotNull]
            public string HeroStatAbilID { get; set; }

            [NotNull]
            public string HeroStatName { get; set; }

        }

        public partial class herostatlearn
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_herostatlearn_1", Order = 0)]
            public Int64 HeroStatLearnID { get; set; }

            [NotNull]
            public Int32 FK_HeroStatInfoID { get; set; }

            [NotNull]
            public Int32 FK_GamePlayerDetailID { get; set; }

            [NotNull]
            public Int32 LearnCount { get; set; }

        }

        public partial class herotype
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_herotype_1", Order = 0)]
            public Int64 HeroTypeID { get; set; }

            [NotNull]
            public string HeroUnitTypeID { get; set; }

        }

        public partial class herotypename
        {
            [Unique(Name = "sqlite_autoindex_herotypename_1", Order = 0)]
            [NotNull]
            public Int64 HeroTypeNameID { get; set; }

            [Indexed(Name = "IX_fk_HeroTypeName_HeroType", Order = 0)]
            [NotNull]
            public Int32 FK_HeroTypeID { get; set; }

            [NotNull]
            public string Language { get; set; }

            [NotNull]
            public string HeroName { get; set; }

            [NotNull]
            public string HeroTitle { get; set; }

        }

        public partial class iteminfo
        {
            [Key]
            public Int32 ItemID { get; set; }

            [NotNull]
            public string ItemTypeID { get; set; }

            [NotNull]
            public string ItemName { get; set; }

            [NotNull]
            public Int32 ItemCost { get; set; }

        }

        public partial class player
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_player_1", Order = 0)]
            public Int64 PlayerID { get; set; }

            [NotNull]
            public Int32 FK_ServerID { get; set; }

            [NotNull]
            public string PlayerName { get; set; }

            [NotNull]
            public DateTime RegDate { get; set; }

            [NotNull]
            public Boolean IsBanned { get; set; }

            [NotNull]
            public DateTime LastUpdatedOn { get; set; }

            [NotNull]
            public string LastUpdatedBy { get; set; }

            public DateTime? BannedOn { get; set; }

            public DateTime? UnbannedOn { get; set; }

        }

        public partial class playerherostat
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_playerherostat_1", Order = 0)]
            public Int64 PlayerHeroStatID { get; set; }

            [NotNull]
            public Int32 FK_PlayerID { get; set; }

            [NotNull]
            public Int32 FK_ServerID { get; set; }

            [NotNull]
            public Int32 FK_HeroTypeID { get; set; }

            [NotNull]
            public Int32 HeroPlayCount { get; set; }

            [NotNull]
            public Int32 TotalHeroKills { get; set; }

            [NotNull]
            public Int32 TotalHeroDeaths { get; set; }

            [NotNull]
            public Int32 TotalHeroAssists { get; set; }

        }

        public partial class playerstat
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_playerstat_1", Order = 0)]
            public Int64 PlayerStatID { get; set; }

            [NotNull]
            public Int32 FK_PlayerID { get; set; }

            [NotNull]
            public Int32 FK_ServerID { get; set; }

            [NotNull]
            public Int32 Win { get; set; }

            [NotNull]
            public Int32 Loss { get; set; }

            [NotNull]
            public Int32 PlayCount { get; set; }

            [NotNull]
            public Int32 ELO { get; set; }

            [NotNull]
            public Int32 Score { get; set; }

        }

        public partial class ranking
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_ranking_1", Order = 0)]
            public Int64 RankID { get; set; }

            [NotNull]
            public Int32 FK_PlayerID { get; set; }

            [NotNull]
            public Int32 FK_ServerID { get; set; }

            [NotNull]
            public Int32 PlayerStatID { get; set; }

            [NotNull]
            public Int32 PlayerRank { get; set; }

        }

        public partial class server
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_server_1", Order = 0)]
            public Int64 ServerID { get; set; }

            [NotNull]
            public string ServerName { get; set; }

            [NotNull]
            public Boolean IsServiced { get; set; }

        }

        public partial class webusers
        {
            [Key, AutoIncrement]
            [Unique(Name = "sqlite_autoindex_webusers_1", Order = 0)]
            public Int64 WebUserID { get; set; }

            [NotNull]
            public string UserName { get; set; }

            [NotNull]
            public string Password { get; set; }

            [NotNull]
            public Boolean IsAdmin { get; set; }

            [NotNull]
            public DateTime CreatedOn { get; set; }

            [NotNull]
            public DateTime ModifiedOn { get; set; }

            [NotNull]
            public string Salt { get; set; }

            [NotNull]
            public string EmailAddress { get; set; }

        }
    }
}