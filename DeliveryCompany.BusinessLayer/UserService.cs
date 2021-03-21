using DeliveryCompany.BusinessLayer.Distances;
using DeliveryCompany.DataLayer;
using DeliveryCompany.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
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
        public Task<User> GetDriverAsync(string email, string password);
        void UpdatingCoordinatesOfExistingUsersInDatabase();
        void UpdatingCoordinatesOfExistingRecipientsInDatabase();
        bool CheckingIfDriverExists(int courierId);
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

        public async Task AddAsync(User newUser)
        {
            var user = CoordinateAssignment(newUser);

            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
        }

        private User CoordinateAssignment(User user)
        {
            var locationCoordinates = _locationService.ChangeLocationToCoordinates(user);
            user.lat = locationCoordinates.Lat;
            user.lon = locationCoordinates.Lon;

            return user;
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
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return context.Users
                    .AsQueryable()
                    .Where(x => x.Type == TypeOfUser.Driver && (x.lat != 999 || x.lon != 999))
                    .ToList();
            }
        }

        public async Task<User> GetDriverAsync(string email, string password)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return await context.Users
                    .AsQueryable()
                    .Where(x => x.Type == TypeOfUser.Driver && x.Email == email && x.Password == password)
                    .FirstOrDefaultAsync();
            }
        }

        public bool CheckingIfDriverExists(int courierId)
        {
            using(var context = _deliveryCompanyDbContextFactoryMethod())
            {
                var courier = context.Users
                    .FirstOrDefault(x => x.Id == courierId && x.Type == TypeOfUser.Driver);
                
                if (courier == null)
                {
                    return false;
                }

                return true;
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

                    _packageService.UpdateAsync(package);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Package recipient addresses no {package.Id} in the database does not exist! Check it!");
                }
            }
        }
    }
}