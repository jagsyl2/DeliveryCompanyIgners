using DeliveryCompany.BusinessLayer.Distances;
using DeliveryCompany.BusinessLayer.Models;
using DeliveryCompany.BusinessLayer.Serializers;
using DeliveryCompany.BusinessLayer.SpaceTimeProviders;
using DeliveryCompany.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeliveryCompany.BusinessLayer
{
    public interface IWaybillsService
    {
        void CreateWaybills();
    }

    public class WaybillsService : IWaybillsService
    {
        private ILocationService _locationService;
        private IVehicleService _vehicleService;
        private IPackageService _packageService;
        private IJsonSerializer _jsonSerializer;
        private IUserService _userService;
        private ITimeProvider _fastForwardTimeProvider;

        public WaybillsService(
                ILocationService locationService,
                IVehicleService vehicleService,
                IPackageService packageService,
                IJsonSerializer jsonSerializer,
                IUserService userService,
                ITimeProvider fastForwardTimeProvider)
        {
            _locationService = locationService;
            _vehicleService = vehicleService;
            _packageService = packageService;
            _jsonSerializer = jsonSerializer;
            _userService = userService;
            _fastForwardTimeProvider = fastForwardTimeProvider;
        }

        public void CreateWaybills()            //tworzenie listy przewozowej
        {
            var todaysPackages = PreparingTodaysParcelsForShippin();        //przygotowanie do wysyłki dzisiejszych paczek
            if (todaysPackages==null)
            {
                return;
            }

            try
            {
                SerializeWaybills(todaysPackages);                          //serializację wykonuje dla każdego samochoddu po kolei, wyszukując na liście paczek z danego dnia czy jakaś została przypisana do samochodu

                _packageService.UpdatePackages(todaysPackages, StateOfPackage.Given);   //po stworzeniu list przewozowych aktualizuję w bazie danych status na "nadana" oraz dopisuje nr samochodu, którym podrużują
            }
            catch (Exception)
            {                                                               //w przypadku problemów dostaje informację, że nie wygenerował listu przewozowego
                Console.WriteLine();
                Console.WriteLine("\tWaybill info: No waybill generated.");
            }
        }

        public List<Package> PreparingTodaysParcelsForShippin()
        {
            var packages = _packageService.GetPackagesWithStatus(StateOfPackage.AwaitingPosting);      //pobieram paczki, których status jest "oczek. na nadanie". w przypadku braku paczek, nie tworzymy listy.
            if (packages.Count < 1)
            {
                return null;
            }

            var couriers = _userService.GetAllDrivers();                        //pobieram  listę kierowców, firma jest mała, więc rekrutacja też powolna i może ich jeszcze brakować, wtedy list przewozowych nie robimy. 
            if (couriers.Count < 1)
            {
                Console.WriteLine();
                Console.WriteLine("\tWaybill info: We haven't got couriers!");
                return null;
            }

            var vehiclesLoadCapacity = GetAllVehiclesLoadCapacity();            //tworze słownik samochodów i ich ładowności - dzięki niej będę na bieżąco kontrolowała ile miejsca zostało na kolejne paczki.
            if (vehiclesLoadCapacity.Count < 1)                                 //jak nie kupimy samochodów to możemy mieć problem z działalnością jako kurierzy, ale trzeba się na wszystko przygotować
            {
                Console.WriteLine();
                Console.WriteLine("\tWaybill info: We haven't got vehicles!");
                return null;
            }

            return AssignPackagesToCouriers(packages, couriers, vehiclesLoadCapacity);      //przypisujemy paczki do kurierów
        }

        public List<Package> AssignPackagesToCouriers(List<Package> packages, List<User> couriers, Dictionary<int, int> vehiclesLoadCapacity)
        {
            var todaysPackages = new List<Package>();
            var vehicleRange = GetAllVehiclesRange();
            var courierLocationAlongTheWay = GetAllCouriersLocationCoordinates();

            foreach (var package in packages)
            {
                var distances = _locationService.CountDistancesFromPackageToCouriers(couriers, package);         //każdej paczce, która oczekuje na nadanie obliczam odległości od wszystkich kurierów 
                if (distances.Count == 0)
                {
                    continue;
                }

                Vehicle vehicle;
                int courierId;
                do
                {
                    courierId = FindTheNearestCourier(distances);                                           //wybieram kuriera, który jest najbliżej nadanej paczki

                    vehicle = _vehicleService.GetVehicleAsync(courierId).Result;                                            //jeśli mam kuriera, który jest najbliżej, odnajduję jego samochód
                    if (vehicle == null)                                                                        //zabezpieczam się też na wypadek gdyby kurier był na tyle nowy, że jeszcze nie dano mu samochodu
                    {
                        distances.Remove(courierId);
                    }
                    if (distances.Count == 0)                                                                   //warunek, który zadziała jeśli z jakiegoś powodu żaden kurier nie będzie miał przypisanego samochodu 
                    {
                        Console.WriteLine();
                        Console.WriteLine($"\tWaybill info for {package.Id}: We are very sorry, our company is just developing in your region.");
                        break;
                    }
                } while (vehicle==null);

                if (vehicle == null)
                {
                    continue;
                }

                var currentLoadCapacity = vehiclesLoadCapacity[vehicle.Id];
                if (currentLoadCapacity < (int)package.Size)                                                    //jeśli paczka nie miesci się do samochodu, zostawiam ją na kolejny dzień
                {
                    continue;
                }

                var currentCuriersLocation = courierLocationAlongTheWay[courierId];
                if (currentCuriersLocation.FirstPackageForCourier == true)
                {
                    var firstPackageSender = new LocationCoordinates() { Lat = package.Sender.lat, Lon = package.Sender.lon };
                    var firstPackageRecipient = new LocationCoordinates() { Lat = package.RecipientLat, Lon = package.RecipientLon };

                    var vehicleRangeWithPackage = AddedDistancesBetweenTwoPlacesForFirstPackage(currentCuriersLocation, firstPackageSender, firstPackageRecipient);

                    var distance = vehicleRangeWithPackage.Sum();
                    if (distance > vehicleRange[vehicle.Id])
                    {
                        continue;
                    }

                    vehicleRange[vehicle.Id] -= vehicleRangeWithPackage[0]; 

                    UpdateCourierLocationsAlongTheWayForFirstPackage(courierLocationAlongTheWay, courierId, firstPackageSender, firstPackageRecipient);
                }
                else
                {
                    var packageSender = new LocationCoordinates() { Lat = package.Sender.lat, Lon = package.Sender.lon };
                    var packageRecipient = new LocationCoordinates() { Lat = package.RecipientLat, Lon = package.RecipientLon };

                    List<double> vehicleRangeWithPackage = AddedDistancesBetweenTwoPlaces(currentCuriersLocation, packageSender, packageRecipient);

                    var distance = vehicleRangeWithPackage.Sum();
                    if (distance > vehicleRange[vehicle.Id])
                    {
                        continue;
                    }

                    ReductionVehicleRange(vehicleRange, vehicle, vehicleRangeWithPackage);
                    UpdateCourierLocationsAlongTheWay(courierLocationAlongTheWay, courierId, packageSender, packageRecipient);
                }

                vehiclesLoadCapacity[vehicle.Id] -= (int)package.Size;                                          //jeśli się paczka mieści to zmniejszam dzisiejszą wolną przestrzeń w samochodzie

                package.VehicleNumber = vehicle.Id;                                                             //wszystko jest ok, paczka ma przypisany nr samochodu, którym będzie podróżowała
                package.NameOfWaybill = $"{vehicle.DriverId}_{_fastForwardTimeProvider.Now.ToString("yyyy-MM-dd")}";

                todaysPackages.Add(package);                                                                    //tworzę listę paczek, które dzisiaj będą podróżowały
            }

            return todaysPackages;
        }

        private static void ReductionVehicleRange(Dictionary<int, double> vehicleRange, Vehicle vehicle, List<double> vehicleRangeWithPackage)
        {
            vehicleRange[vehicle.Id] -= vehicleRangeWithPackage[0];
            vehicleRange[vehicle.Id] -= vehicleRangeWithPackage[2];
        }

        private static void UpdateCourierLocationsAlongTheWay(Dictionary<int, CourierLocationsAlongTheWay> courierLocationAlongTheWay, int courierId, LocationCoordinates packageSender, LocationCoordinates packageRecipient)
        {
            courierLocationAlongTheWay[courierId].CourierCurrentLocation = packageSender;
            courierLocationAlongTheWay[courierId].RecipientCurrentPackage = packageRecipient;
        }

        private List<double> AddedDistancesBetweenTwoPlaces(CourierLocationsAlongTheWay currentCuriersLocation, LocationCoordinates packageSender, LocationCoordinates packageRecipient)
        {
            var vehicleRangeWithPackage = new List<double>();
            vehicleRangeWithPackage.Add(_locationService.GetDistanceBetweenTwoPlaces(currentCuriersLocation.CourierCurrentLocation, packageSender));
            vehicleRangeWithPackage.Add(_locationService.GetDistanceBetweenTwoPlaces(packageSender, currentCuriersLocation.RecipientFirstPackage));
            vehicleRangeWithPackage.Add(_locationService.GetDistanceBetweenTwoPlaces(currentCuriersLocation.RecipientFirstPackage, packageRecipient));
            vehicleRangeWithPackage.Add(_locationService.GetDistanceBetweenTwoPlaces(packageRecipient, currentCuriersLocation.StartingPlace));
            return vehicleRangeWithPackage;
        }

        private static void UpdateCourierLocationsAlongTheWayForFirstPackage(Dictionary<int, CourierLocationsAlongTheWay> courierLocationAlongTheWay, int courierId, LocationCoordinates firstPackageSender, LocationCoordinates firstPackageRecipient)
        {
            courierLocationAlongTheWay[courierId].CourierCurrentLocation = firstPackageSender;
            courierLocationAlongTheWay[courierId].RecipientFirstPackage = firstPackageRecipient;
            courierLocationAlongTheWay[courierId].RecipientCurrentPackage = firstPackageRecipient;
            courierLocationAlongTheWay[courierId].FirstPackageForCourier = false;
        }

        private List<double> AddedDistancesBetweenTwoPlacesForFirstPackage(CourierLocationsAlongTheWay currentCuriersLocation, LocationCoordinates firstPackageSender, LocationCoordinates firstPackageRecipient)
        {
            var vehicleRangeWithPackage = new List<double>();

            vehicleRangeWithPackage.Add(_locationService.GetDistanceBetweenTwoPlaces(currentCuriersLocation.StartingPlace, firstPackageSender));
            vehicleRangeWithPackage.Add(_locationService.GetDistanceBetweenTwoPlaces(firstPackageSender, firstPackageRecipient));
            vehicleRangeWithPackage.Add(_locationService.GetDistanceBetweenTwoPlaces(firstPackageRecipient, currentCuriersLocation.StartingPlace));
            
            return vehicleRangeWithPackage;
        }

        private Dictionary<int, CourierLocationsAlongTheWay> GetAllCouriersLocationCoordinates()
        {
            var courierLocationAlongTheWay = new Dictionary<int, CourierLocationsAlongTheWay>();
            var couriers = _userService.GetAllDrivers();

            foreach (var courier in couriers)
            {
                var courierLocations = new CourierLocationsAlongTheWay()
                {
                    StartingPlace = new LocationCoordinates { Lat = courier.lat, Lon = courier.lon },
                };
                courierLocationAlongTheWay.Add(courier.Id, courierLocations);
            }

            return courierLocationAlongTheWay;
        }

        private Dictionary<int, double> GetAllVehiclesRange()
        {
            var vehiclesRange = new Dictionary<int, double>();
            var vehicles = _vehicleService.GetAllVehicles();
            const int courierWorkingTime = 10;

            foreach (var vehicle in vehicles)
            {
                var range = vehicle.AverageSpeed*courierWorkingTime;
                vehiclesRange.Add(vehicle.Id, range);
            }

            return vehiclesRange;
        }

        private Dictionary<int, int> GetAllVehiclesLoadCapacity()
        {
            var vehiclesLoadCapacity = new Dictionary<int, int>();
            var vehicles = _vehicleService.GetAllVehicles();

            foreach (var vehicle in vehicles)
            {
                vehiclesLoadCapacity.Add(vehicle.Id, vehicle.LoadCapacity);
            }

            return vehiclesLoadCapacity;
        }

        private void SerializeWaybills(List<Package> packages)
        {
            var slnPath = AppDomain.CurrentDomain.BaseDirectory;
            var path = Directory.CreateDirectory(slnPath + "shipping_lists");

            var vehicles = _vehicleService.GetAllVehicles();
            foreach (var vehicle in vehicles)
            {
                var date = _fastForwardTimeProvider.Now.ToString("yyyy-MM-dd");
                var filePath = Path.Combine(path.FullName, $"{vehicle.DriverId}_{date}.json");

                var waybill = packages
                        .Where(x => x.VehicleNumber == vehicle.Id)
                        .ToList();

                _jsonSerializer.Serialize(filePath, waybill);
            }
        }

        public int FindTheNearestCourier(Dictionary<int, double> distances)
        {
            return distances
                .Where(x => x.Value == distances.Values.Min())
                .Select(x => x.Key)
                .FirstOrDefault();
        }
    }
}
