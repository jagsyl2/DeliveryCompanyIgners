using DeliveryCompany.BusinessLayer.SpaceTimeProviders;
using DeliveryCompany.DataLayer;
using DeliveryCompany.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryCompany.BusinessLayer
{
    public class CourierRatingsService
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
                var rating = packages.Sum(x=> x.CourierRating)/packages.Count;

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

        public List<Rating> GetListOfRatings(int courierId)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return context.Ratings
                    .AsQueryable()
                    .Where(x => x.UserId == courierId)
                    .OrderBy(x=>x.DateTime)
                    .ToList();
            }
        }

        public void Print(int courierId, List<Rating> ratings)
        {
            Console.WriteLine($"Ratings for the courier {courierId}");

            foreach (var rating in ratings)
            {
                Console.WriteLine($"{rating.Id}. Waybill dated: {rating.DateTime} - rating: {rating.CouriersRating}");
            }
            Console.WriteLine();
        }
    }
}
