using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wc3_fate_west_data_access_layer;
using wc3_fate_west_data_access_layer.Data;
using wc3_fate_west_web.Models;
using wc3_fate_west_web.Pages;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace wc3_fate_west_web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GameDetailController : ControllerBase
    {
        private readonly FateWestDbContext context;
        public GameDetailController(FateWestDbContext context)
        {
            this.context = context;
        }
        //gameDetail
        // GET: api/GameDetailData/id
        [HttpGet("{id}")]
        public ActionResult<GameDetailData> GetGameDetail(int id)
        {
            GameDetailSL gds = new GameDetailSL(context);
            var GameDetail = gds.GetGameDetail(id);

            if (GameDetail == null)
            {
                return NotFound();
            }

            return GameDetail;
        }
    }

}