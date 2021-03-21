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
    public interface ICourierRatingsService
    { 
        void CountAverageRatingForWaybill();
        Task<List<Rating>> GetListOfRatingsAsync(int courierId);
    }

    public class CourierRatingsService : ICourierRatingsService
    {
        private readonly IVehicleService _vehicleService;
        private readonly ITimeProvider _fastForwardTimeProvider;
        private readonly IPackageService _packageService;
        private readonly Func<IDeliveryCompanyDbContext> _deliveryCompanyDbContextFactoryMethod;

        public CourierRatingsService(
            IVehicleService vehicleService,
            ITimeProvider fastForwardTimeProvider,
            IPackageService packageService,
            Func<IDeliveryCompanyDbContext> deliveryCompanyDbContextFactoryMethod)
        {
            _vehicleService = vehicleService;
            _fastForwardTimeProvider = fastForwardTimeProvider;
            _packageService = packageService;
            _deliveryCompanyDbContextFactoryMethod = deliveryCompanyDbContextFactoryMethod;
        }

        public void CountAverageRatingForWaybill()
        {
            var vehicles = _vehicleService.GetAllVehicles();

            var date = _fastForwardTimeProvider.Now.ToString("yyyy-MM-dd");

            var todayWaybill = new List<Rating>();

            foreach (var vehicle in vehicles)
            {
                var waybill = $"{vehicle.DriverId}_{date}";
                var packages = _packageService.GetPackagesTodaysDelivered(waybill);
                if (packages.Count==0)
                {
                    continue;
                }
                var rating = Math.Round((double)packages.Sum(x => x.CourierRating) / packages.Count,1);
                

                var ratingForWaybill = new Rating()
                {
                    UserId = vehicle.DriverId,
                    DateTime = _fastForwardTimeProvider.Now,
                    CouriersRating = rating,
                };

                todayWaybill.Add(ratingForWaybill);
            }

            Add(todayWaybill);
        }

        private void Add(List<Rating> todayWaybills)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                foreach (var waybill in todayWaybills)
                {
                    context.Ratings.Add(waybill);
                }

                context.SaveChanges();
            }
        }

        public async Task<List<Rating>> GetListOfRatingsAsync(int courierId)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return await context.Ratings
                    .AsQueryable()
                    .Where(x => x.UserId == courierId)
                    .OrderByDescending(x => x.DateTime)
                    .ToListAsync();
            }
        }
    }
}
