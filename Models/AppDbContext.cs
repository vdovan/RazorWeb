using Microsoft.EntityFrameworkCore;

namespace RazorWeb.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsbuilder)
        {
            base.OnConfiguring(optionsbuilder);
        }

        public DbSet<Article>? articles {get; set;}
    }
}