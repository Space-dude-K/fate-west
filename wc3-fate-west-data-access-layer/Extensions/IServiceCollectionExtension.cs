using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using wc3_fate_west_data_access_layer.Db;

namespace wc3_fate_west_data_access_layer.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddSqliteDatabaseConnectorForFateWest(this IServiceCollection services)
        {
            services.AddDbContext<FateWestDbContext>(options => options.UseSqlite(new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build().GetSection("ConnectionStrings")["FateWestDbConnectionString"]));
            services.AddTransient<IReplayDataWriter, ReplayDataWriter>();

            return services;
        }
        public static IServiceCollection AddSqliteDatabaseConnectorForGhost(this IServiceCollection services)
        {
            services.AddDbContext<GhostDbContext>(options => options.UseSqlite(new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build().GetSection("ConnectionStrings")["GhostDbConnectionString"]));
            services.AddTransient<IGhostDataReceiver, GhostDataReceiver>();

            return services;
        }
    }
}