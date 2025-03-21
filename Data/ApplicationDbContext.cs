using Microsoft.EntityFrameworkCore;
using Recipesbook.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recipesbook.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Bakery> Bakery { get; set; }
        public DbSet<Dairy> Dairy { get; set; }
        public DbSet<Fruits> Fruits { get; set; }
        public DbSet<Meat> Meat { get; set; }
        public DbSet<Vegatables> Vegetables { get; set; }
        public DbSet<GrainAndPasta> GrainAndPasta { get; set; }

        public DbSet<FloursANDBakingIngredients> FloursANDBakingIngredients { get; set; }
        public DbSet<SpicesAndCondiments> SpicesAndCondiments { get; set; }
    }

}
