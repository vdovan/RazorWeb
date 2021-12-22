using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RazorWeb.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsbuilder)
        {
            base.OnConfiguring(optionsbuilder);
        }

        public DbSet<Article>? articles {get; set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach(var entityType in builder.Model.GetEntityTypes())
            {
                string? tableName = entityType.GetTableName();
                if(!string.IsNullOrEmpty(tableName) && tableName.ToLower().StartsWith("aspnet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
                
            }
        }
    }
}