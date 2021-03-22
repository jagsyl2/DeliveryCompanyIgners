using DeliveryCompany.AppForDrivers.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System.Text;
using DeliveryCompany.AppForDrivers.Distances;
using DeliveryCompany.DataLayer.Models;
using DeliveryCompany.AppForDrivers.SpaceTimeProviders;
using Unity;

namespace DeliveryCompany.AppForDrivers
{
    class Program
    {
        private readonly ITimeProvider    _fastForwardTimeProvider;
        private readonly ILocationService _locationService;
        private readonly IIoHelper        _ioHelper;
        private readonly IPackageServices _packageServices;

        private bool _exit = false;

        private User _user = null;
        private List<Package> _waybill = null;
        private Vehicle _vehicle = null;
        private DateTime _startTimeOfWork;

        private const double _minutes = 60.0d;

        private List<WaybillItem> _waybillItems = new List<WaybillItem>();
        
        static void Main(string[] args)
        {
            var container = new UnityDiContainerProvider().GetContainer();

            container.Resolve<Program>().Run();
        }

        public Program(
            ITimeProvider fastForwardTimeProvider,
            ILocationService locationService,
            IIoHelper        ioHelper,
            IPackageServices packageServices)
        {
            _fastForwardTimeProvider = fastForwardTimeProvider;
            _locationService = locationService;
            _ioHelper = ioHelper;
            _packageServices = packageServices;
        }

        void Run()
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
                    _startTimeOfWork = _packageServices.GetStartTime();
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
                Console.WriteLine("4. Rating display");
                Console.WriteLine("5. Exit");

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
                        RatingDisplay();
                        break;
                    case 5:
                        _exit = true;
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            } 
            while (!_exit);
        }

        private void RatingDisplay()
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync($@"http://localhost:10500/api/waybills/rating/{_user.Id}").Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var ratingList = JsonConvert.DeserializeObject<List<Rating>>(responseText);
                    if (ratingList.Count==0)
                    {
                        Console.WriteLine("No rated waybills!");
                        return;
                    }
                    _ioHelper.Print(_user.Id, ratingList);
                }
                else
                {
                    Console.WriteLine($"Failed. Status code: {response.StatusCode}");
                }
            }
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
                     _packageServices.MarkDeliveredPackage(_waybill, _waybillItems);
                    break;
                case 2:
                    return;
                default:
                    Console.WriteLine("Unknown options. Try again...");
                    break;
            }
        }

        private void CheckingTheRoute()
        {
            if (_waybill == null)
            {
                Console.WriteLine("The waybill has not been collected.");
                return;
            }

            var currentLocation = new LocationCoordinates {Lat = _user.Lat, Lon = _user.Lon };

            var deliveryTime = _startTimeOfWork;

            foreach (var package in _waybill)
            {
                var distance = _locationService.GetDistanceBetweenTwoPlaces(currentLocation, package.Sender.Lat, package.Sender.Lon);
                deliveryTime = CountEstimatedDeliveryTime(deliveryTime, package, distance, TypeOfPackages.ForPickup);

                currentLocation.Lat = package.Sender.Lat;
                currentLocation.Lon = package.Sender.Lon;
            }

            foreach (var package in _waybill)
            {
                var distance = _locationService.GetDistanceBetweenTwoPlaces(currentLocation, package.RecipientLat, package.RecipientLon);
                deliveryTime = CountEstimatedDeliveryTime(deliveryTime, package, distance, TypeOfPackages.ToBeDelivered);
                
                currentLocation.Lat = package.RecipientLat;
                currentLocation.Lon = package.RecipientLon;
            }

            foreach (var item in _waybillItems)
            {
                _ioHelper.PrintPackageAlongTheRoute(item);
            }
        }

        private DateTime CountEstimatedDeliveryTime(DateTime deliveryTime, Package package, double distance, TypeOfPackages typeOfPackages)
        {
            var waybillItem = new WaybillItem
            {
                Package = package,
                Distance = distance,
                Time = Math.Round(distance / (double)_vehicle.AverageSpeed * _minutes),
                TypeOfPackage = typeOfPackages,
            };
            waybillItem.EstimatedDeliveryTime = deliveryTime.AddMinutes(waybillItem.Time);

            _waybillItems.Add(waybillItem);

            deliveryTime = waybillItem.EstimatedDeliveryTime;
            return deliveryTime;
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
                    if (responseObject.Count==0)
                    {
                        Console.WriteLine("There are no packages to develop today.");
                        return;
                    }
                    
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
                    Console.WriteLine($"The package has been updated.");
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
