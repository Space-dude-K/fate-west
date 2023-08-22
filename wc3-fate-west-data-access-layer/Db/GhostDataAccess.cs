using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace wc3_fate_west_data_access_layer.Db
{
    public class GhostDataAccess
    {
        public partial class admins
        {
            [Key, AutoIncrement]
            public Int64 id { get; set; }

            [NotNull]
            public String name { get; set; }

            [NotNull]
            public String server { get; set; }

        }

        public partial class bans
        {
            [Key, AutoIncrement]
            public Int64 id { get; set; }

            [NotNull]
            public String server { get; set; }

            [NotNull]
            public String name { get; set; }

            public String ip { get; set; }

            [NotNull]
            public String date { get; set; }

            public String gamename { get; set; }

            [NotNull]
            public String admin { get; set; }

            public String reason { get; set; }

        }

        public partial class config
        {
            [Key]
            public String name { get; set; }

            [NotNull]
            public String value { get; set; }

        }

        public partial class dotagames
        {
            [Key, AutoIncrement]
            public Int64 id { get; set; }

            [NotNull]
            public Int64 gameid { get; set; }

            [NotNull]
            public Int64 winner { get; set; }

            [NotNull]
            public Int64 min { get; set; }

            [NotNull]
            public Int64 sec { get; set; }

        }

        public partial class dotaplayers
        {
            [Key, AutoIncrement]
            public Int64 id { get; set; }

            [Indexed(Name = "idx_gameid_colour", Order = 0)]
            [NotNull]
            public Int64 gameid { get; set; }

            [Indexed(Name = "idx_gameid_colour", Order = 1)]
            [NotNull]
            public Int64 colour { get; set; }

            [NotNull]
            public Int64 kills { get; set; }

            [NotNull]
            public Int64 deaths { get; set; }

            [NotNull]
            public Int64 creepkills { get; set; }

            [NotNull]
            public Int64 creepdenies { get; set; }

            [NotNull]
            public Int64 assists { get; set; }

            [NotNull]
            public Int64 gold { get; set; }

            [NotNull]
            public Int64 neutralkills { get; set; }

            [NotNull]
            public String item1 { get; set; }

            [NotNull]
            public String item2 { get; set; }

            [NotNull]
            public String item3 { get; set; }

            [NotNull]
            public String item4 { get; set; }

            [NotNull]
            public String item5 { get; set; }

            [NotNull]
            public String item6 { get; set; }

            [NotNull]
            public String hero { get; set; }

            [NotNull]
            public Object newcolour { get; set; }

            [NotNull]
            public Object towerkills { get; set; }

            [NotNull]
            public Object raxkills { get; set; }

            [NotNull]
            public Object courierkills { get; set; }

        }

        public partial class downloads
        {
            [Key, AutoIncrement]
            public Int64 id { get; set; }

            [NotNull]
            public String map { get; set; }

            [NotNull]
            public Int64 mapsize { get; set; }

            [NotNull]
            public String datetime { get; set; }

            [NotNull]
            public String name { get; set; }

            [NotNull]
            public String ip { get; set; }

            [NotNull]
            public Int64 spoofed { get; set; }

            [NotNull]
            public String spoofedrealm { get; set; }

            [NotNull]
            public Int64 downloadtime { get; set; }

        }

        public partial class gameplayers
        {
            [Key, AutoIncrement]
            public Int64 id { get; set; }

            [Indexed(Name = "idx_gameid", Order = 0)]
            [NotNull]
            public Int64 gameid { get; set; }

            [NotNull]
            public String name { get; set; }

            [NotNull]
            public String ip { get; set; }

            [NotNull]
            public Int64 spoofed { get; set; }

            [NotNull]
            public Int64 reserved { get; set; }

            [NotNull]
            public Int64 loadingtime { get; set; }

            [NotNull]
            public Int64 left { get; set; }

            [NotNull]
            public String leftreason { get; set; }

            [NotNull]
            public Int64 team { get; set; }

            [NotNull]
            public Int64 colour { get; set; }

            [NotNull]
            public String spoofedrealm { get; set; }

        }

        public partial class games
        {
            [Key, AutoIncrement]
            public Int64 id { get; set; }

            [NotNull]
            public String server { get; set; }

            [NotNull]
            public String map { get; set; }

            [NotNull]
            public String datetime { get; set; }

            [NotNull]
            public String gamename { get; set; }

            [NotNull]
            public String ownername { get; set; }

            [NotNull]
            public Int64 duration { get; set; }

            [NotNull]
            public Int64 gamestate { get; set; }

            [NotNull]
            public String creatorname { get; set; }

            [NotNull]
            public String creatorserver { get; set; }

        }

        public partial class lobbies
        {
            [Key]
            [Unique(Name = "sqlite_autoindex_lobbies_1", Order = 0)]
            public Int64 lobbyId { get; set; }

            public Int64? gameCounter { get; set; }

            public String realm { get; set; }

            public String lobbyName { get; set; }

            public Int64? lobbyStatus { get; set; }

            public String lobbyDateTime { get; set; }

            public Int64? lobbyType { get; set; }

        }

        public partial class slots
        {
            [Key, AutoIncrement]
            public Int64 id { get; set; }
            public Int64? lobbyId { get; set; }

            public Int64? slotId { get; set; }

            public Int64? playerId { get; set; }

            public Int64? playerTeam { get; set; }

            public String playerName { get; set; }

            public Int64? playerColour { get; set; }

            public Int64? playerPing { get; set; }

        }

        public partial class w3mmdplayers
        {
            [Key, AutoIncrement]
            public Int64 id { get; set; }

            [NotNull]
            public String category { get; set; }

            [NotNull]
            public Int64 gameid { get; set; }

            [NotNull]
            public Int64 pid { get; set; }

            [NotNull]
            public String name { get; set; }

            [NotNull]
            public String flag { get; set; }

            [NotNull]
            public Int64 leaver { get; set; }

            [NotNull]
            public Int64 practicing { get; set; }

        }

        public partial class w3mmdvars
        {
            [Key, AutoIncrement]
            public Int64 id { get; set; }

            [NotNull]
            public Int64 gameid { get; set; }

            [NotNull]
            public Int64 pid { get; set; }

            [NotNull]
            public String varname { get; set; }

            public Int64? value_int { get; set; }

            public Double? value_real { get; set; }

            public String value_string { get; set; }

        }
    }
}