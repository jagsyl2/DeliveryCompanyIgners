using DeliveryCompany.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace DeliveryCompany.DataLayer
{
    public interface IDeliveryCompanyDbContext : IDisposable
    {
        DbSet<Package> Packages { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Vehicle> Vehicles { get; set; }
        DatabaseFacade Database { get; }

        int SaveChanges();
    }

    public class DeliveryCompanyDbContext : DbContext, IDeliveryCompanyDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.;Database=DeliveryCompanyIgners_Dev4;Trusted_Connection=True");
        }
    }
}