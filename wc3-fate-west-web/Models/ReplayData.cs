using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wc3_fate_west_data_access_layer;

namespace wc3_fate_west_web.Models
{
    public class ReplayData
    {
        public static void Initialize(FateWestDbContext db)
        {
            if (db != null)
            {
                //var dbServer = db.server.FirstOrDefault(x => x.ServerName == serverName);

                db.SaveChanges();
            }
        }
    }
}
