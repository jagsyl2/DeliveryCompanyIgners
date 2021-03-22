﻿using DeliveryCompany.BusinessLayer.Distances;
using DeliveryCompany.BusinessLayer.Notifications;
using DeliveryCompany.BusinessLayer.SpaceTimeProviders;
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
        Task UpdateAsync(Package package);
        void UpdatePackages(List<Package> packages, StateOfPackage stateOfPackage);
        public void UpdatePackages(List<Package> packages, StateOfPackage stateOfPackage, int couriersRating);
        Task UpdateByIdWithNotifyAsync(int id, Package package);
        void UpdatePackagesOnAutomaticWaybill(List<Package> packages, StateOfPackage stateOfPackage);
        List<Package> GetPackagesWithStatus(StateOfPackage stateOfPackage);
        List<Package> GetPackagesWithStatusOnAutomaticWaybill(StateOfPackage stateOfPackage);
        List<Package> GetPackagesWithStatusOnManualWaybill(StateOfPackage stateOfPackage);
        Task<List<Package>> GetPackagesOnCouriersWaybillAsync(int id);
        public List<Package> GetPackagesTodaysDelivered(string waybill);
        List<Package> GetAllPackagesWithoutCoordinates();
        Task<Package> GetPackageAsync(int id);
    }

    public class PackageService : IPackageService
    {
        private Func<IDeliveryCompanyDbContext> _deliveryCompanyDbContextFactoryMethod;
        private readonly ILocationService _locationService;
        private readonly ITimeProvider _fastForwardTimeProvider;
        private readonly INotificationService _notificationService;

        public PackageService(
            Func<IDeliveryCompanyDbContext> deliveryCompanyDbContextFactoryMethod,
            ILocationService locationService,
            ITimeProvider fastForwardTimeProvider,
            INotificationService notificationService)
        {
            _deliveryCompanyDbContextFactoryMethod = deliveryCompanyDbContextFactoryMethod;
            _locationService = locationService;
            _fastForwardTimeProvider = fastForwardTimeProvider;
            _notificationService = notificationService;
        }

        public async Task AddAsync(Package newPackage)
        {
            var package = CoordinateAssignment(newPackage);

            newPackage.Number = Guid.NewGuid();
            newPackage.DateOfRegistration = _fastForwardTimeProvider.Now;
            newPackage.State = StateOfPackage.AwaitingPosting;

            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                context.Packages.Add(newPackage);
                await context.SaveChangesAsync();
            }
        }

        private Package CoordinateAssignment(Package package)
        {
            var locationCoordinates = _locationService.ChangeLocationToCoordinates(
                                            package.RecipientCity,
                                            package.RecipientPostCode,
                                            package.RecipientStreet,
                                            package.RecipientStreetNumber);

            package.RecipientLat = locationCoordinates.Lat;
            package.RecipientLon = locationCoordinates.Lon;

            return package;
        }

        public async Task UpdateAsync(Package package)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                context.Packages.Update(package);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateByIdWithNotifyAsync(int id, Package package)
        {
            package.Id = id;

            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                context.Packages.Update(package);
                await context.SaveChangesAsync();
            }

            _notificationService.NotifyOfPackageDelivery(package);
        }

        public void UpdatePackages(List<Package> packages, StateOfPackage stateOfPackage)
        {
            foreach (var package in packages)
            {
                package.State = stateOfPackage;
                UpdateAsync(package).Wait();
            }
        }

        public void UpdatePackages(List<Package> packages, StateOfPackage stateOfPackage, int couriersRating)
        {
            foreach (var package in packages)
            {
                package.CourierRating = couriersRating;
                package.State = stateOfPackage;
                UpdateAsync(package).Wait();
            }
        }

        public void UpdatePackagesOnAutomaticWaybill(List<Package> packages, StateOfPackage stateOfPackage)
        {
            foreach (var package in packages)
            {
                package.State = stateOfPackage;
                package.ModeWaybill = ModeOfWaybill.Automatic;
                UpdateAsync(package).Wait();
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

        public List<Package> GetPackagesWithStatusOnAutomaticWaybill(StateOfPackage stateOfPackage)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return context.Packages
                    .Include(x => x.Sender)
                    .Where(x => (x.State == stateOfPackage && x.ModeWaybill==ModeOfWaybill.Automatic && (x.RecipientLat != 999 || x.RecipientLon != 999)))
                    .ToList();
            }
        }

        public List<Package> GetPackagesWithStatusOnManualWaybill(StateOfPackage stateOfPackage)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return context.Packages
                    .Include(x => x.Sender)
                    .Where(x => (x.State == stateOfPackage && x.ModeWaybill == ModeOfWaybill.Manual && (x.RecipientLat != 999 || x.RecipientLon != 999)))
                    .ToList();
            }
        }

        public async Task<List<Package>> GetPackagesOnCouriersWaybillAsync(int id)
        {
            List<Package> waybill;
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                waybill = await context.Packages
                    .Include(x => x.Sender)
                    .Where(x => (x.VehicleNumber==id && (x.State == StateOfPackage.Given || x.State == StateOfPackage.OnTheWay)))
                    .ToListAsync();
            }

            foreach (var package in waybill)
            {
                package.Sender.Password = "Unavailable";
            }

            return waybill;
        }

        public List<Package> GetPackagesTodaysDelivered(string waybill)
        {
            List<Package> packages;
            using(var context = _deliveryCompanyDbContextFactoryMethod())
            {
                packages = context.Packages
                    .AsQueryable()
                    .Where(x=>x.NameOfWaybill == waybill)
                    .ToList();
            }
            return packages;
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

        public async Task<Package> GetPackageAsync(int id)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return await context.Packages
                    .AsQueryable()
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
        }
    }
}
