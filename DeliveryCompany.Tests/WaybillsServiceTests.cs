using DeliveryCompany.BusinessLayer;
using DeliveryCompany.BusinessLayer.Distances;
using DeliveryCompany.BusinessLayer.Serializers;
using DeliveryCompany.BusinessLayer.SpaceTimeProviders;
using DeliveryCompany.DataLayer.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace DeliveryCompany.Tests
{
    public class WaybillsServiceTests
    {
        [TestCase(20.5, 150.6, 97.0, 1)]
        [TestCase(10.5, 10.6, 10.1, 3)]
        [TestCase(679.05, 545.8, 976.5, 2)]
        public void FindTheNearestCourier_From3Distances_ReturnKeyForTheNearestDistance(double distance1, double distance2, double distance3, int result)
        {
            Dictionary<int, double> distances = new Dictionary<int, double>();
            distances.Add(1, distance1);
            distances.Add(2, distance2);
            distances.Add(3, distance3);

            var locationServiceMock = new Mock<ILocationService>();
            var vehicleServiceMock = new Mock<IVehicleService>();
            var packageServiceMock = new Mock<IPackageService>();
            var jsonSerializerMock = new Mock<IJsonSerializer>();
            var userServiceMock = new Mock<IUserService>();
            var timeProvider = new Mock<ITimeProvider>();

            var service = new WaybillsService(
                locationServiceMock.Object,
                vehicleServiceMock.Object,
                packageServiceMock.Object,
                jsonSerializerMock.Object,
                userServiceMock.Object,
                timeProvider.Object);

            int nearestDistance = service.FindTheNearestCourier(distances);

            nearestDistance.Should().Be(result);
        }

        [Test]
        public void PreparingTodaysParcelsForShippin_3PackagesAndNoDrivers_ReturnNullListPackages()
        {
            List<Package> packages = new List<Package>();
            packages.Add(new Package(1, PackageSize.Average));
            packages.Add(new Package(2, PackageSize.Large));
            packages.Add(new Package(3, PackageSize.Small));

            List<User> users = new List<User>();

            var packageServiceMock = new Mock<IPackageService>();
            packageServiceMock
                .Setup(x => x.GetPackagesWithStatus(StateOfPackage.AwaitingPosting))
                .Returns(packages);

            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(x => x.GetAllDrivers())
                .Returns(users);

            var locationServiceMock = new Mock<ILocationService>();
            var vehicleServiceMock = new Mock<IVehicleService>();
            var jsonSerializerMock = new Mock<IJsonSerializer>();
            var timeProvider = new Mock<ITimeProvider>();

            var service = new WaybillsService(
                locationServiceMock.Object,
                vehicleServiceMock.Object,
                packageServiceMock.Object,
                jsonSerializerMock.Object,
                userServiceMock.Object,
                timeProvider.Object);

            List<Package> todaysPackage = service.PreparingTodaysParcelsForShippin();

            todaysPackage.Should().BeNull();
        }

        [Test]
        public void AssignPackagesToCouriers_GivePackageAndDriverWithoutVehile_ReturnEmptyList()
        {
            List<Package> packages = new List<Package>();
            packages.Add(new Package()
            {
                Id = 3,
                RecipientLat = 54,
                RecipientLon = 18,
                Sender = new User()
                {
                    Lat = 53.5,
                    Lon = 19
                },
                Size = PackageSize.Average
            });

            List<User> users = new List<User>();
            users.Add(new User() { Id = 5, Street = "Œliska", StreetNumber = "28C", City = "Gdynia" });

            Dictionary<int, int> vehiclesCapacity = new Dictionary<int, int>();
            Dictionary<int, double> distance = new Dictionary<int, double>();
            distance.Add(3, 20);

            List<Vehicle> vehicles = new List<Vehicle>();

            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(x => x.GetAllDrivers())
                .Returns(users);

            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock
                .Setup(x => x.GetAllVehicles())
                .Returns(vehicles);

            vehicleServiceMock
                .Setup(x => x.GetVehicleAsync(5).Result)
                .Returns(new Vehicle());

            var locationServiceMock = new Mock<ILocationService>();
            locationServiceMock
                .Setup(y => y.CountDistancesFromPackageToCouriers(users, It.IsAny<Package>()))
                .Returns(distance);

            var packageServiceMock = new Mock<IPackageService>();
            var jsonSerializerMock = new Mock<IJsonSerializer>();
            var timeProvider = new Mock<ITimeProvider>();

            var service = new WaybillsService(
                locationServiceMock.Object,
                vehicleServiceMock.Object,
                packageServiceMock.Object,
                jsonSerializerMock.Object,
                userServiceMock.Object,
                timeProvider.Object);

            List<Package> todaysPackage = service.AssignPackagesToCouriers(packages, users, vehiclesCapacity);

            todaysPackage.Should().BeEmpty();
        }

        [Test]
        public void AssignPackagesToCouriers_Give1Package_ReturnListWith1Position()
        {
            List<Package> packages = new List<Package>();
            packages.Add(new Package()
            {
                Id = 3,
                RecipientLat = 54,
                RecipientLon = 18,
                Sender = new User()
                {
                    Lat = 53.5,
                    Lon = 19 
                },
                Size = PackageSize.Average
            });

            List<User> users = new List<User>();
            users.Add(new User() { Id = 5, Lat = 52, Lon = 21 });
            users.Add(new User() { Id = 6, Lat = 53, Lon = 20 });

            Dictionary<int, double> distance = new Dictionary<int, double>();
            distance.Add(5, 20);
            distance.Add(6, 150);

            List<Vehicle> vehicles = new List<Vehicle>();
            vehicles.Add(new Vehicle() { Id = 5, AverageSpeed = 120 });
            vehicles.Add(new Vehicle() { Id = 6, AverageSpeed = 100 });

            Dictionary<int, int> vehiclesCapacity = new Dictionary<int, int>();
            vehiclesCapacity.Add(5, 200);

            LocationCoordinates firstPackageSender = new LocationCoordinates();

            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(x => x.GetAllDrivers())
                .Returns(users);

            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock
                .Setup(x => x.GetAllVehicles())
                .Returns(vehicles);

            vehicleServiceMock
                .Setup(x => x.GetVehicleAsync(5).Result)
                .Returns(new Vehicle() { Id = 5 });

            var locationServiceMock = new Mock<ILocationService>();
            locationServiceMock
                .Setup(y => y.CountDistancesFromPackageToCouriers(users, It.IsAny<Package>()))
                .Returns(distance);

            var packageServiceMock = new Mock<IPackageService>();
            var jsonSerializerMock = new Mock<IJsonSerializer>();
            var timeProvider = new Mock<ITimeProvider>();

            var service = new WaybillsService(
                locationServiceMock.Object,
                vehicleServiceMock.Object,
                packageServiceMock.Object,
                jsonSerializerMock.Object,
                userServiceMock.Object,
                timeProvider.Object);

            List<Package> todaysPackage = service.AssignPackagesToCouriers(packages, users, vehiclesCapacity);

            todaysPackage.Should().HaveCount(1);
        }

        [Test]
        public void PreparingTodaysParcelsForShippin_NoPackages_ReturnNullListPackages()
        {
            List<Package> packages = new List<Package>();

            List<User> users = new List<User>();
            users.Add(new User() { Id = 1, Type = TypeOfUser.Driver });

            var packageServiceMock = new Mock<IPackageService>();
            packageServiceMock
                .Setup(x => x.GetPackagesWithStatus(StateOfPackage.AwaitingPosting))
                .Returns(packages);

            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(x => x.GetAllDrivers())
                .Returns(users);

            var locationServiceMock = new Mock<ILocationService>();
            var vehicleServiceMock = new Mock<IVehicleService>();
            var jsonSerializerMock = new Mock<IJsonSerializer>();
            var timeProvider = new Mock<ITimeProvider>();

            var service = new WaybillsService(
                locationServiceMock.Object,
                vehicleServiceMock.Object,
                packageServiceMock.Object,
                jsonSerializerMock.Object,
                userServiceMock.Object,
                timeProvider.Object);

            List<Package> todaysPackage = service.PreparingTodaysParcelsForShippin();

            todaysPackage.Should().BeNull();
        }

        [Test]
        public void AssignPackagesToCouriers_Give4Packages_ReturnListWith3Positions()
        {
            List<Package> packages = new List<Package>();
            packages.Add(new Package() { Id = 1, Size = PackageSize.Average, Sender = new User() { Lat = 53.5, Lon = 19 }});
            packages.Add(new Package() { Id = 2, Size = PackageSize.Large, Sender = new User() { Lat = 53.5, Lon = 19 } });
            packages.Add(new Package() { Id = 3, Size = PackageSize.Average, Sender = new User() { Lat = 53.5, Lon = 19 } });
            packages.Add(new Package() { Id = 4, Size = PackageSize.Small, Sender = new User() { Lat = 53.5, Lon = 19 } });

            List<User> users = new List<User>();
            users.Add(new User() { Id = 5, Street = "Œliska", StreetNumber = "28C", City = "Gdynia" });
            users.Add(new User() { Id = 6, Street = "Niska", StreetNumber = "5", City = "Bydgoszcz" });

            Dictionary<int, double> distance = new Dictionary<int, double>();
            distance.Add(5, 20);
            distance.Add(6, 150);

            Dictionary<int, int> vehiclesCapacity = new Dictionary<int, int>();
            vehiclesCapacity.Add(5, 220);

            List<Vehicle> vehicles = new List<Vehicle>();
            vehicles.Add(new Vehicle() { Id = 5, AverageSpeed = 120 });
            vehicles.Add(new Vehicle() { Id = 6, AverageSpeed = 100 });

            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(x => x.GetAllDrivers())
                .Returns(users);

            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock
                .Setup(x => x.GetVehicleAsync(5).Result)
                .Returns(new Vehicle() { Id = 5 });

            vehicleServiceMock
                .Setup(x => x.GetAllVehicles())
                .Returns(vehicles);

            var locationServiceMock = new Mock<ILocationService>();
            locationServiceMock
                .Setup(y => y.CountDistancesFromPackageToCouriers(users, It.IsAny<Package>()))
                .Returns(distance);

            var packageServiceMock = new Mock<IPackageService>();
            var jsonSerializerMock = new Mock<IJsonSerializer>();
            var timeProvider = new Mock<ITimeProvider>();

            var service = new WaybillsService(
                locationServiceMock.Object,
                vehicleServiceMock.Object,
                packageServiceMock.Object,
                jsonSerializerMock.Object,
                userServiceMock.Object,
                timeProvider.Object);

            List<Package> todaysPackage = service.AssignPackagesToCouriers(packages, users, vehiclesCapacity);

            todaysPackage.Should().HaveCount(3);
            Assert.IsTrue(todaysPackage.Contains(packages[0]));
            Assert.IsTrue(todaysPackage.Contains(packages[1]));
            Assert.IsFalse(todaysPackage.Contains(packages[2]));
            Assert.IsTrue(todaysPackage.Contains(packages[3]));
        }
    }
}
