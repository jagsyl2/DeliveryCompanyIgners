using DeliveryCompany.DataLayer;
using DeliveryCompany.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryCompany.BusinessLayer
{
    public interface IPackageService
    {
        Task AddAsync(Package package);
        void Update(Package package);
        void UpdatePackages(List<Package> packages, StateOfPackage stateOfPackage);
        List<Package> GetPackagesWithStatus(StateOfPackage stateOfPackage);
        Task<List<Package>> GetPackagesOnCouriersWaybillAsync(int id);
        List<Package> GetAllPackagesWithoutCoordinates();
    }

    public class PackageService : IPackageService
    {
        private Func<IDeliveryCompanyDbContext> _deliveryCompanyDbContextFactoryMethod;

        public PackageService(Func<IDeliveryCompanyDbContext> deliveryCompanyDbContextFactoryMethod)
        {
            _deliveryCompanyDbContextFactoryMethod = deliveryCompanyDbContextFactoryMethod;
        }

        public async Task AddAsync(Package package)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                context.Packages.Add(package);
                await context.SaveChangesAsync();
            }
        }

        public void Update(Package package)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                context.Packages.Update(package);
                context.SaveChanges();
            }
        }

        public void UpdatePackages(List<Package> packages, StateOfPackage stateOfPackage)
        {
            foreach (var package in packages)
            {
                package.State = stateOfPackage;
                Update(package);
            }
        }

        public List<Package> GetPackagesWithStatus(StateOfPackage stateOfPackage)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return context.Packages
                    .Include(x => x.Sender)
                    .Where(x => (x.State == stateOfPackage && (x.RecipientLat != 999 || x.RecipientLon != 999)))
                    .ToList();
            }
        }

        public async Task<List<Package>> GetPackagesOnCouriersWaybillAsync(int id)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                var vehicle = context.Vehicles.FirstOrDefault(x => x.DriverId == id);

                return await context.Packages
                    .Include(x => x.Sender)
                    .Where(x => ((x.State == StateOfPackage.Given || x.State == StateOfPackage.OnTheWay) && x.VehicleNumber==vehicle.Id))
                    .ToListAsync();
            }
        }

        public List<Package> GetAllPackagesWithoutCoordinates()
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return context.Packages
                    .AsQueryable()
                    .Where(x => (x.RecipientLat == 999 || x.RecipientLon == 999))
                    .ToList();
            }
        }
    }
}
