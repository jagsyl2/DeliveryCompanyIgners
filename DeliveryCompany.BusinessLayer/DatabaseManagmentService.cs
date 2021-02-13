using DeliveryCompany.BusinessLayer.Distances;
using DeliveryCompany.DataLayer;
using System;

namespace DeliveryCompany.BusinessLayer
{
    public interface IDatabaseManagmentService
    {
        void EnsureDatabaseCreation();
        public void UpdatingCoordinatesOfExistingUsersInDatabase();
        public void UpdatingCoordinatesOfExistingRecipientsInDatabase();
    }

    public class DatabaseManagmentService : IDatabaseManagmentService
    {
        private readonly Func<IDeliveryCompanyDbContext> _deliveryCompanyDbContextFactoryMethod;
        private readonly UserService _userService;
        private readonly PackageService _packageService;
        private readonly LocationService _locationService;

        public DatabaseManagmentService(
            Func<IDeliveryCompanyDbContext> deliveryCompanyDbContextFactoryMethod,
            UserService userService,
            PackageService packageService,
            LocationService locationService)
        {
            _deliveryCompanyDbContextFactoryMethod = deliveryCompanyDbContextFactoryMethod;
            _userService = userService;
            _packageService = packageService;
            _locationService = locationService;
        }

        public void EnsureDatabaseCreation()
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                context.Database.EnsureCreated();
            }
        }

        public void UpdatingCoordinatesOfExistingUsersInDatabase()
        {
            var users = _userService.GetAllUsersWithoutCoordinates();

            foreach (var user in users)
            {
                var userCoordinate = _locationService.ChangeLocationToCoordinates(user);
                user.lat = userCoordinate.Lat;
                user.lon = userCoordinate.Lon;

                _userService.Update(user);
            }
        }

        public void UpdatingCoordinatesOfExistingRecipientsInDatabase()
        {
            var packages = _packageService.GetAllPackagesWithoutCoordinates();

            foreach (var package in packages)
            {
                var packageCoordinate = _locationService.ChangeLocationToCoordinates(package.RecipientCity, package.RecipientPostCode, package.RecipientStreet, package.RecipientStreetNumber);
                package.RecipientLat = packageCoordinate.Lat;
                package.RecipientLon = packageCoordinate.Lon;

                _packageService.Update(package);
            }
        }
    }
}
