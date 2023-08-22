using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace wc3_fate_west_data_access_layer.Data.Wc3_Ghost
{
    public class Lobby
    {
        private int lobbyId;
        private string gameCounter;
        private string realm;
        private string lobbyName;
        private GameStatus gameStatus;
        private LobbyType lobbyType;
        private DateTime lobbyDateTime;
        public enum GameStatus
        {
            Open,
            InProgress,
            Finished,
            Processed
        }
        public enum LobbyType
        {
            Public,
            Private
        }
        private List<Slot> lobbySlots;
        public int LobbyId { get => lobbyId; set => lobbyId = value; }
        public string GameCounter { get => gameCounter; set => gameCounter = value; }
        public string Realm { get => realm; set => realm = value; }
        public string LobbyName { get => lobbyName; set => lobbyName = value; }
        public string SetGameStatus { set => gameStatus = (GameStatus)Enum.Parse(typeof(GameStatus), value); }
        public GameStatus GetGameStatus { get => gameStatus; }
        public string SetLobbyType { set => lobbyType = (LobbyType)Enum.Parse(typeof(LobbyType), value); }
        public LobbyType GetLobbyType { get => lobbyType; }
        public List<Slot> LobbySlots { get => lobbySlots; set => lobbySlots = value; }
        public string LobbyCurrentPlayers { get => lobbySlots.Where(l => l.IsHuman).Count().ToString() + "/" + lobbySlots.Count().ToString(); }
        public string SetLobbyDateTime { set => lobbyDateTime = DateTime.ParseExact(value, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture); }
        public DateTime LobbyDateTime { get => lobbyDateTime; }
    }
}