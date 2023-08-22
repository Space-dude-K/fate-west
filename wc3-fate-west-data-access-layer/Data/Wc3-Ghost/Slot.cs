using System.Drawing;

namespace wc3_fate_west_data_access_layer.Data.Wc3_Ghost
{
    public class Slot
    {
        private string slotId;
        private int playerId;
        private int playerTeam;
        private string playerName;
        private Color playerColor;
        private string playerPing;
        private Stats playerStats;

        public string SlotId { get => slotId; set => slotId = value; }
        public int PlayerId { get => playerId; set => playerId = value; }
        public int PlayerTeam { get => playerTeam; set => playerTeam = value; }
        public string PlayerName { get => playerName; set => playerName = value; }
        public Color PlayerColor { get => playerColor; }
        public string SetPlayerColor { set => playerColor = ConvertStringToDiscordColor(value); }
        public string PlayerPing { get => playerPing; set => playerPing = value; }
        public bool IsHuman { get => playerId > 1; }
        public Stats PlayerStats { get => playerStats; set => playerStats = value; }

        private Color ConvertStringToDiscordColor(string rawStringColor)
        {
            switch (rawStringColor)
            {
                case "0":
                    return Color.Red;
                case "1":
                    return Color.Blue;
                case "2":
                    return Color.Teal;
                case "3":
                    return Color.Purple;
                case "4":
                    return Color.FromArgb(255, 255, 0);
                case "5":
                    return Color.Orange;
                case "6":
                    return Color.Green;
                case "7":
                    return Color.FromArgb(255, 153, 204);
                case "8":
                    return Color.LightGray;
                case "9":
                    return Color.FromArgb(153, 204, 255);
                case "10":
                    return Color.DarkGreen;
                case "11":
                    return Color.FromArgb(02, 51, 0);
                default:
                    return Color.LightGray;
            }
        }
    }
}