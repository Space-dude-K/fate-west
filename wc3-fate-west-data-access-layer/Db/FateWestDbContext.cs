using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using static wc3_fate_west_data_access_layer.Db.FateWestDataAccess;

namespace wc3_fate_west_data_access_layer
{
    public class FateWestDbContext : DbContext
    {
        public FateWestDbContext(DbContextOptions<FateWestDbContext> options) : base(options) { }

        public DbSet<server> server { get; set; }
        public DbSet<game> game { get; set; }
        public DbSet<gameplayerdetail> gameplayerdetail { get; set; }
        public DbSet<gameitempurchase> gameitempurchase { get; set; }
        //iteminfo
        public DbSet<iteminfo> iteminfo { get; set; }
        //herostatinfo
        public DbSet<herostatinfo> herostatinfo { get; set; }
        //herostatlearn
        public DbSet<herostatlearn> herostatlearn { get; set; }
        //godshelpinfo
        public DbSet<godshelpinfo> godshelpinfo { get; set; }
        //godshelpuse
        public DbSet<godshelpuse> godshelpuse { get; set; }
        //commandsealuses
        public DbSet<commandsealuses> commandsealuses { get; set; }
        //attributeinfo
        public DbSet<attributeinfo> attributeinfo { get; set; }
        //attributelearn
        public DbSet<attributelearn> attributelearn { get; set; }
        //herotype
        public DbSet<herotype> herotype { get; set; }
        // herotypename
        public DbSet<herotypename> herotypename { get; set; }
        //playerherostat
        public DbSet<playerherostat> playerherostat { get; set; }
        //playerstat
        public DbSet<playerstat> playerstat { get; set; }
        //player
        public DbSet<player> player { get; set; }
    }
    /*
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FateWestDbContext>
    {
        //public FateWestDbContext CreateDbContext(string[] args)
        public FateWestDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile(@Directory.GetCurrentDirectory() + "/wc3-fate-west-web/appsettings.json")
                .AddJsonFile(@"E:\work\Projects\Fate\wc3-fate-west\wc3-fate-west-web\appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<FateWestDbContext>();
            var connectionString = configuration.GetConnectionString("FateWestDbConnectionString");
            builder.UseSqlite(connectionString);
            return new FateWestDbContext(builder.Options);
        }
    }
    */
}
