using Microsoft.EntityFrameworkCore;
using SQLite;
using System;
using static wc3_fate_west_data_access_layer.Db.GhostDataAccess;

namespace wc3_fate_west_data_access_layer.Db
{
    public class GhostDbContext : DbContext
    {
        public GhostDbContext(DbContextOptions<GhostDbContext> options) : base(options) { }
        public DbSet<admins> admins { get; set; }
        public DbSet<bans> bans { get; set; }
        public DbSet<config> configs { get; set; }
        public DbSet<downloads> downloads { get; set; }
        public DbSet<gameplayers> gameplayers { get; set; }
        public DbSet<games> games { get; set; }
        public DbSet<lobbies> lobbies { get; set; }
        public DbSet<slots> slots { get; set; }
    }
}