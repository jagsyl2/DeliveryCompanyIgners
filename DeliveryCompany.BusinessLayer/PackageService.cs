using DeliveryCompany.DataLayer;
using DeliveryCompany.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryCompany.BusinessLayer
{
    public interface IPackageService
    {
        public void Add(Package package);
        public void Update(Package package);
        public List<Package> CheckPackagesAwaitingPosting();
    }

    public class PackageService : IPackageService
    {
        public void Add(Package package)
        {
            using (var context = new DeliveryCompanyDbContext())
            {
                context.Packages.Add(package);
                context.SaveChanges();
            }
        }

        public void Update(Package package)
        {
            using (var context = new DeliveryCompanyDbContext())
            {
                context.Update(package);
                context.SaveChanges();
            }
        }

        public List<Package> CheckPackagesAwaitingPosting()
        {
            using (var context = new DeliveryCompanyDbContext())
            {
                return context.Packages
                    .Include(x => x.Sender)
                    .Where(x => x.State == StateOfPackage.AwaitingPosting)
                    .ToList();
            }
        }
    }
}
