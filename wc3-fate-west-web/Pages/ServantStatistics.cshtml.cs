using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using wc3_fate_west_data_access_layer;
using wc3_fate_west_web.Models;

namespace wc3_fate_west_web.Pages
{
    public class ServantStatisticsModel : PageModel
    {
        private readonly FateWestDbContext fateWestDbContext;
        public List<ServantStatisticsViewModel> SsvModel { get => new StatisticsSL(fateWestDbContext).GetServantStatistics(); }
        public ServantStatisticsModel(FateWestDbContext fateWestDbContext)
        {
            this.fateWestDbContext = fateWestDbContext;
            Debug.WriteLine("WIN RATIO: " + SsvModel.FirstOrDefault().WinRatio + " Color: " + SsvModel.FirstOrDefault().ProgressBarColor);
        }
    }
}