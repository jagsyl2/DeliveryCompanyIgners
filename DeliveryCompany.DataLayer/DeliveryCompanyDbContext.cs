using DeliveryCompany.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryCompany.DataLayer
{
    public class DeliveryCompanyDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.;Database=DeliveryCompanyIgners_Dev3;Trusted_Connection=True");
        }
    }
}