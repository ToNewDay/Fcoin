using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace DataAccess
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder build)
        {
            string connectstr= ConfigurationManager.AppSettings["ConnectionStrings"];
            build.UseMySQL("Server=localhost;database=fcoin;uid=root;pwd=123;");
        }

        public DbSet<TRADE_DATA> TRADE_DATA { get; set; }
        public DbSet<SYMBOL_DATA> SYMBOL_DATA { get; set; }

        public DbSet<TRADE_INFO_PER_MIN> TRADE_INFO_PER_MIN { get; set; }

        public DbSet<REWARD_DATA> REWARD_DATA { get; set; }

        public DbSet<TARGET_ORDER> TARGET_ORDER { get; set; }
    }
}