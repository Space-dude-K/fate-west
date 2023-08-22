using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NLog;
using wc3_fate_west_data_access_layer;
using wc3_fate_west_web.Models;

namespace wc3_fate_west_web.Pages
{
    public class PlayerStatsPageModel : PageModel
    {
        PlayerStatsPageViewModel pspvm;
        private readonly FateWestDbContext fateWestDbContext;

        public PlayerStatsPageViewModel Pspvm { get => pspvm; }
        public PlayerStatsPageModel(FateWestDbContext fateWestDbContext)
        {
            this.fateWestDbContext = fateWestDbContext;
        }
        public void OnGetPlayerStats1(PlayerStatsPageViewModel pspvm)
        {
            this.pspvm = pspvm;
            Debug.WriteLine("PageM -> PSS: " + pspvm.LastGameID + " " + pspvm.HasFoundUser + " PHero sum: " + pspvm.PlayerHeroStatSummaryData.Count);
        }
        public void OnGetPlayerStats(string serverName, string playerName)
        {
            PlayerStatSL pssl = new PlayerStatSL(fateWestDbContext);
            pspvm = pssl.GetPlayerStatSummary(playerName, serverName, int.MaxValue);
            Debug.WriteLine("PageM -> PSS: " + pspvm.LastGameID + " " + pspvm.HasFoundUser + " PHero sum: " + pspvm.PlayerHeroStatSummaryData.Count);
        }
    }
}