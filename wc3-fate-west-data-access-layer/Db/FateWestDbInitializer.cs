using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace wc3_fate_west_data_access_layer.Db
{
    public class FateWestDbInitializer
    {
        public FateWestDbInitializer(ServiceCollection services, string jsonConfigPath)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(jsonConfigPath)
                .Build();

            services.AddDbContext<FateWestDbContext>(options => options.UseSqlite(configuration.GetConnectionString("FateWestDbConnectionString")));
            services.AddDbContext<GhostDbContext>(options => options.UseSqlite(configuration.GetConnectionString("GhostDbConnectionString")));
        }
    }
}