using System.Collections.Generic;
using Newtonsoft.Json;
// ReSharper disable ClassNeverInstantiated.Global

namespace wc3_fate_west_data_access_layer.Data
{
    public class GameListData
    {
        [JsonProperty(PropertyName = "Lobby")]
        public GameListLobbyData Lobby { get; set; }
        [JsonProperty(PropertyName = "Progress")]
        public List<GameListProgressData> ProgressList { get; set; } 
        public string Server { get; set; }
    }

    public class GameListPlayerData
    {
        public string HeroIconURL { get; set; }
        public int SlotNumber { get; set; }
        public int TeamNumber { get; set; }
        public string PlayerName { get; set; }
        public string Server { get; set; }
        public string Ping { get; set; }
        public bool IsConnected { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
    }

    public class GameListLobbyData
    {
        [JsonProperty(PropertyName = "LobbyPlayers")]
        public List<GameListPlayerData> PlayerDataList { get; set; }
        public bool IsAvailable { get; set; }
        public int PlayerCount { get; set; }
        public int SlotSize { get; set; }
        public string GameName { get; set; }
        public string Owner { get; set; }
    }

    public class GameListKDAData
    {
        public int PlayerID { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
    }

    public class GameListProgressData
    {
        [JsonProperty(PropertyName = "ProgressPlayers")]
        public List<GameListPlayerData> PlayerDataList { get; set; }
        public List<GameListPlayerData> Team1DataList { get; set; } = new List<GameListPlayerData>();
        public List<GameListPlayerData> Team2DataList { get; set; } = new List<GameListPlayerData>();
        [JsonProperty(PropertyName = "FRSKills")]
        public List<GameListKDAData> PlayerKills { get; set; }
        [JsonProperty(PropertyName = "FRSDeaths")]
        public List<GameListKDAData> PlayerDeaths { get; set; }
        [JsonProperty(PropertyName = "FRSAssists")]
        public List<GameListKDAData> PlayerAssists { get; set; }
        public int Team1Wins { get; set; }
        public int Team2Wins { get; set; }
        [JsonProperty(PropertyName = "FRSEvents")]
        public List<string> FRSEventList { get; set; }
        public int GameNumber { get; set; }
        public int PlayerCount { get; set; }
        public int SlotSize { get; set; }
        public string Server { get; set; }
        public string GameName { get; set; }
        public int GameDuration { get; set; }
    }
}
