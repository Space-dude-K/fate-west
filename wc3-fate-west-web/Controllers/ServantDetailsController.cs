using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using wc3_fate_west_data_access_layer;
using wc3_fate_west_data_access_layer.Data;
using wc3_fate_west_web.Models;

namespace wc3_fate_west_web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ServantDetailsController : ControllerBase
    {
        private readonly FateWestDbContext context;
        public ServantDetailsController(FateWestDbContext context)
        {
            this.context = context;
        }
        // GET: id
        [HttpGet("{id}")]
        public ActionResult<List<ServantStatisticsDetailViewModel>> GetServantDetail(int id)
        {
            StatisticsSL sSl = new StatisticsSL(context);
            ServantDetailSL sdSl = new ServantDetailSL(sSl);
            List<ServantStatisticsDetailViewModel> ssdvModel = sdSl.GetServantDetail(id);

            if (ssdvModel == null)
            {
                return NotFound();
            }

            return ssdvModel;
        }
    }
}
