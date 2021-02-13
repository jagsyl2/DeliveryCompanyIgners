using DeliveryCompany.DataLayer;
using DeliveryCompany.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryCompany.BusinessLayer
{
    public interface IPackageService
    {
        public void Add(Package package);
        public void Update(Package package);
        public void UpdatePackages(List<Package> packages, StateOfPackage stateOfPackage);
        public List<Package> GetPackagesWithStatus(StateOfPackage stateOfPackage);
    }

    public class PackageService : IPackageService
    {
        private Func<IDeliveryCompanyDbContext> _deliveryCompanyDbContextFactoryMethod;

        public PackageService(Func<IDeliveryCompanyDbContext> deliveryCompanyDbContextFactoryMethod)
        {
            _deliveryCompanyDbContextFactoryMethod = deliveryCompanyDbContextFactoryMethod;
        }

        public void Add(Package package)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                context.Packages.Add(package);
                context.SaveChanges();
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
                    .Where(x => x.State == stateOfPackage)
                    .ToList();
            }
        }

        public List<Package> GetAllPackagesWithoutCoordinates()
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return context.Packages
                    .Where(x => (x.RecipientLat == 999 || x.RecipientLon == 999))
                    .ToList();
            }
        }
    }
}
