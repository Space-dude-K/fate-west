using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using wc3_fate_west_data_access_layer;
using wc3_fate_west_web.Models;

namespace wc3_fate_west_web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlayerStatsAjaxController : ControllerBase
    {
        private readonly FateWestDbContext fateWestDbContext;
        public PlayerStatsAjaxController(FateWestDbContext fateWestDbContext)
        {
            this.fateWestDbContext = fateWestDbContext;
        }
        // Get["/PlayerStats/{server}/{playerName}"]
        // GET: api/GameDetailData/id
        [HttpGet("{serverName}/{playerName}/{gameId}/{heroUnitTypeId}")]
        public ActionResult<List<PlayerGameSummaryViewModel>> GetGameDetailWithFilter(string serverName, string playerName, int gameId, string heroUnitTypeId)
        {
            PlayerStatSL pssl = new PlayerStatSL(fateWestDbContext);
            List<PlayerGameSummaryViewModel> gameSummaryData = pssl.GetPlayerGameSummary(playerName, serverName, gameId, heroUnitTypeId);

            Debug.WriteLine("Ajax pgs: " + gameSummaryData.Count);

            if (gameSummaryData == null)
            {
                return NotFound();
            }

            return gameSummaryData;
        }
        [HttpGet("Statistics/{orderType}/{time}")]
        public ActionResult<List<PlayerStatisticsViewModel>> GetStatistics(int orderType, int time)
        {
            StatisticsSL sSl = new StatisticsSL(fateWestDbContext);
            List<PlayerStatisticsViewModel> gameStatistics = sSl.GetPlayerStatistics(orderType, time);

            if (gameStatistics == null)
            {
                return NotFound();
            }

            return gameStatistics;
        }
    }
}