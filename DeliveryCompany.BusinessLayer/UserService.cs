using DeliveryCompany.BusinessLayer.Distances;
using DeliveryCompany.DataLayer;
using DeliveryCompany.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryCompany.BusinessLayer
{
    public interface IUserService
    {
        Task AddAsync(User user);
        List<User> GetAllCustomers();
        List<User> GetAllDrivers();
        public User GetDriver(User user);
        void UpdatingCoordinatesOfExistingUsersInDatabase();
        void UpdatingCoordinatesOfExistingRecipientsInDatabase();
    }

    public class UserService : IUserService
    {
        private readonly Func<IDeliveryCompanyDbContext> _deliveryCompanyDbContextFactoryMethod;
        private readonly ILocationService _locationService;
        private readonly IPackageService _packageService;

        public UserService(
            Func<IDeliveryCompanyDbContext> deliveryCompanyDbContextFactoryMethod,
            ILocationService locationService,
            IPackageService packageService)
        {
            _deliveryCompanyDbContextFactoryMethod = deliveryCompanyDbContextFactoryMethod;
            _locationService = locationService;
            _packageService = packageService;
        }

        public async Task AddAsync(User user)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
        }

        public void Update(User user)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                context.Users.Update(user);
                context.SaveChanges();
            }
        }

        public List<User> GetAllCustomers()
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return context.Users
                    .AsQueryable()
                    .Where(x => x.Type == TypeOfUser.Customer && (x.lat != 999 || x.lon != 999))
                    .ToList();
            }
        }

        public List<User> GetAllDrivers()
        {
            using(var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return context.Users
                    .AsQueryable()
                    .Where(x => x.Type == TypeOfUser.Driver && (x.lat != 999 || x.lon != 999))
                    .ToList();
            }
        }

        public User GetDriver(User user)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return context.Users
                    .AsQueryable()
                    .Where(x => x.Type == TypeOfUser.Driver && x.Email == user.Email && x.Password == user.Password)
                    .FirstOrDefault();
            }
        }

        public List<User> GetAllUsersWithoutCoordinates()
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return context.Users
                    .AsQueryable()
                    .Where(x => (x.lat == 999 || x.lon == 999))
                    .ToList();
            }
        }

        public void UpdatingCoordinatesOfExistingUsersInDatabase()
        {
            var users = GetAllUsersWithoutCoordinates();

            foreach (var user in users)
            {
                try
                {
                    var userCoordinate = _locationService.ChangeLocationToCoordinates(user);
                    user.lat = userCoordinate.Lat;
                    user.lon = userCoordinate.Lon;

                    Update(user);
                }
                catch (Exception)
                {
                    Console.WriteLine($"User no {user.Id} addresses in the database does not exist! Check it!");
                }
            }
        }

        public void UpdatingCoordinatesOfExistingRecipientsInDatabase()
        {
            var packages = _packageService.GetAllPackagesWithoutCoordinates();

            foreach (var package in packages)
            {
                try
                {
                    var packageCoordinate = _locationService.ChangeLocationToCoordinates(
                                                package.RecipientCity,
                                                package.RecipientPostCode,
                                                package.RecipientStreet,
                                                package.RecipientStreetNumber);

                    package.RecipientLat = packageCoordinate.Lat;
                    package.RecipientLon = packageCoordinate.Lon;

                    _packageService.Update(package);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Package recipient addresses no {package.Id} in the database does not exist! Check it!");
                }
            }
        }
    }
}