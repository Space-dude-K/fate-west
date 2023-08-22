using Microsoft.AspNetCore.Mvc;
using wc3_fate_west_data_access_layer;
using wc3_fate_west_web.Models;

namespace wc3_fate_west_web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlayerStatsController : ControllerBase
    {
        private readonly FateWestDbContext fateWestDbContext;
        public PlayerStatsController(FateWestDbContext fateWestDbContext)
        {
            this.fateWestDbContext = fateWestDbContext;
        }
        // Get["/PlayerStats/{server}/{playerName}"]
        [HttpGet("{serverName}/{playerName}")]
        public ActionResult<PlayerStatsPageViewModel> GetGameDetail(string serverName, string playerName)
        {
            if (serverName == null || playerName == null)
            {
                return NotFound();
            }

            return RedirectToPage("/PlayerStatsPage", "PlayerStats", new { serverName, playerName });
        }
    }
}