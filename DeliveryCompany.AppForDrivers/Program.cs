using DeliveryCompany.AppForDrivers.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System.Text;
using DeliveryCompany.AppForDrivers.Distances;
using DeliveryCompany.DataLayer.Models;
using System.Linq;
using DeliveryCompany.AppForDrivers.SpaceTimeProviders;

namespace DeliveryCompany.AppForDrivers
{
    class Program
    {
        private readonly IoHelper _ioHelper = new IoHelper();
        private readonly LocationService _locationService = new LocationService();
        private readonly FastForwardTimeProvider _fastForwardTimeProvider = new FastForwardTimeProvider();

        private bool _exit = false;
        private User _user = null;
        private List<Package> _waybill = null;
        private Vehicle _vehicle = null;
        private const double minutes = 60.0d;
        private List<WaybillItem> waybillItems =new List<WaybillItem>();

        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            do
            {
                var driver = GetDriversDataFromUser();

                GetDriver(driver);
            }
            while (!_exit);
        }

        private void GetDriver(User driver)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync($@"http://localhost:10500/api/users/find?email={driver.Email}&password={driver.Password}").Result;
                var responseText = response.Content.ReadAsStringAsync().Result;
                
                if (response.StatusCode==HttpStatusCode.OK)
                {
                    var responseObject = JsonConvert.DeserializeObject<User>(responseText);
                    _user = responseObject;
                    _vehicle = GetVehicle();
                    Menu();
                }
                else
                {
                    Console.WriteLine($"Failed. Status code: {response.StatusCode}");
                    return;
                }
            }
        }

        private void Menu()
        {
            do
            {
                Console.WriteLine("Choose option:");
                Console.WriteLine("1. Get waybill");
                Console.WriteLine("2. Checking the route");
                Console.WriteLine("3. Manual development of packages");
                Console.WriteLine("4. Exit");

                var option = _ioHelper.GetIntFromUser("Enter option no:");

                switch (option)
                {
                    case 1:
                        GetWaybill();
                        break;
                    case 2:
                        CheckingTheRoute();
                        break;
                    case 3:
                        ManualDevelopmentOfPackages();
                        break;
                    case 4:
                        _exit = true;
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            } 
            while (!_exit);
        }

        private void ManualDevelopmentOfPackages()
        {
            if (_waybill == null)
            {
                Console.WriteLine("The waybill has not been collected.");
                return;
            }

            Console.WriteLine("Avaiable options:");
            Console.WriteLine("1. Mark the delivered package.");
            Console.WriteLine("2. Back to options.");
            var userChoice = _ioHelper.GetIntFromUser("Choose option:");


            switch (userChoice)
            {
                case 1:
                    MarkDeliveredPackage();
                    break;
                case 2:
                    return;
                default:
                    Console.WriteLine("Unknown options. Try again...");
                    break;
            }
        }

        private void MarkDeliveredPackage()
        {
            var deliveredPackageId = _ioHelper.GetIntFromUser("Provide the number of the delivered package:");
            if (!_waybill.Any(x => x.Id == deliveredPackageId))
            {
                Console.WriteLine("There is no such package.");
                return;
            }
            if (_waybill.Any(x => x.Id == deliveredPackageId && x.State==StateOfPackage.DeliveredManually))
            {
                Console.WriteLine("Package has already been delivered.");
            }
            else
            {
                UpdatePackageStatus(deliveredPackageId);
                return;
            }

        }

        private void UpdatePackageStatus(int deliveredPackageId)
        {
            var package = _waybill.FirstOrDefault(x => x.Id == deliveredPackageId);

            package.State = StateOfPackage.DeliveredManually;
            package.DeliveryDate = _fastForwardTimeProvider.Now;

            var content = new StringContent(JsonConvert.SerializeObject(package), Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var response = httpClient.PutAsync($@"http://localhost:10500/api/packages/status/{deliveredPackageId}", content).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine($"Success.");
                    return;
                }
                else
                {
                    Console.WriteLine($"Failed again. Status code: {response.StatusCode}");
                    return;
                }
            }
        }

        private void CheckingTheRoute()
        {
            if (_waybill == null)
            {
                Console.WriteLine("The waybill has not been collected.");
                return;
            }

            var currentLocation = new LocationCoordinates
            {
                Lat = _user.lat,
                Lon = _user.lon,
            };

            foreach (var package in _waybill)
            {
                var distance = _locationService.GetDistanceBetweenTwoPlaces(currentLocation, package.Sender.lat, package.Sender.lon);
                var waybillItem = new WaybillItem
                {
                    Package = package,
                    Distance = distance,
                    Time = Math.Round(distance / (double)_vehicle.AverageSpeed * minutes),
                };
                waybillItems.Add(waybillItem);

                currentLocation.Lat = package.Sender.lat;
                currentLocation.Lon = package.Sender.lon;
            }

            foreach (var package in _waybill)
            {
                var distance = _locationService.GetDistanceBetweenTwoPlaces(currentLocation, package.RecipientLat, package.RecipientLon);
                var waybillItem = new WaybillItem
                {
                    Package = package,
                    Distance = distance,
                    Time = Math.Round(distance / (double)_vehicle.AverageSpeed * minutes),
                };
                waybillItems.Add(waybillItem);

                currentLocation.Lat = package.RecipientLat;
                currentLocation.Lon = package.RecipientLon;
            }

            foreach (var item in waybillItems)
            {
                _ioHelper.PrintPackageAlongTheRoute(item);
            }
        }

        private Vehicle GetVehicle()
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync($@"http://localhost:10500/api/vehicles/{_user.Id}").Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseObject = JsonConvert.DeserializeObject<Vehicle>(responseText);
                    return responseObject;
                }
                else
                {
                    Console.WriteLine($"Failed. Status code: {response.StatusCode}");
                    return null;
                }
            }
        }

        private void GetWaybill()
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync($@"http://localhost:10500/api/packages/waybill/{_vehicle.Id}").Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseObject = JsonConvert.DeserializeObject<List<Package>>(responseText);
                    _waybill = responseObject;

                    foreach (var package in responseObject)
                    {
                        UpdateWaybillMode(package);
                        _ioHelper.PrintPackages(package);
                    }
                }
                else
                {
                    Console.WriteLine($"Failed again. Status code: {response.StatusCode}");
                }
            }
        }

        private void UpdateWaybillMode(Package package)
        {
            if (package.ModeWaybill == ModeOfWaybill.Automatic)
            {
                return;
            }
            
            package.ModeWaybill = ModeOfWaybill.Manual;
            package.State = StateOfPackage.OnTheWay;

            var content = new StringContent(JsonConvert.SerializeObject(package), Encoding.UTF8, "application/json");
            
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.PutAsync($@"http://localhost:10500/api/packages/waybill", content).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine($"Success.");
                }
                else
                {
                    Console.WriteLine($"Failed again. Status code: {response.StatusCode}");
                }
            }
        }

        private User GetDriversDataFromUser()
        {
            Console.WriteLine("Welcome in Igners' Delivery Company!");
            Console.WriteLine("If you want to download the waybill, please log in.");

            var user = new User
            {
                Email = _ioHelper.GetStringFromUser("Enter your e-mail:"),
                Password = _ioHelper.GetStringFromUser("Enter the password:")
            };

            return user;
        }
    }
}
