﻿using DeliveryCompany.AppForDrivers.Models;
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
        private DateTime _startTimeOfWork;
        private const double _minutes = 60.0d;
        private List<WaybillItem> _waybillItems =new List<WaybillItem>();

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
                    _startTimeOfWork = GetStartTime();
                    Menu();
                }
                else
                {
                    Console.WriteLine($"Failed. Status code: {response.StatusCode}");
                    return;
                }
            }
        }

        private DateTime GetStartTime()
        {
            if (0 < _fastForwardTimeProvider.Now.Hour && _fastForwardTimeProvider.Now.Hour< 8)
            {
                return _fastForwardTimeProvider.Now;
            }
            else
            {
                return _fastForwardTimeProvider.Now.Date.AddHours(8);
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
                        _exit = LogOut();
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            } 
            while (!_exit);
        }

        private bool LogOut()
        {
            _user = null;
            _waybill = null;
            _vehicle = null;
            _startTimeOfWork = DateTime.Today.AddHours(8);

            return true;
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
            if (_waybill.Any(x => x.Id == deliveredPackageId && x.ModeWaybill == ModeOfWaybill.Automatic))
            {
                var package = GetPackage(deliveredPackageId);
                if (package == null)
                {
                    Console.WriteLine("There is no such package.");
                    return;
                }
                if (package.State == StateOfPackage.Received)
                {
                    Console.WriteLine("Package has already been delivered.");
                    return;
                }

            }
            else
            {
                UpdatePackageStatus(deliveredPackageId);
                return;
            }

        }

        private Package GetPackage(int deliveredPackageId)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync($@"http://localhost:10500/api/packages/{deliveredPackageId}").Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseObject = JsonConvert.DeserializeObject<Package>(responseText);
                    return responseObject;
                }
                else
                {
                    Console.WriteLine("Something went wrong");
                    return null;
                }
            }
        }

        private void UpdatePackageStatus(int deliveredPackageId)
        {
            var package = _waybill.FirstOrDefault(x => x.Id == deliveredPackageId);

            package.State = StateOfPackage.DeliveredManually;
            package.DeliveryDate = _fastForwardTimeProvider.Now;
            var estimatedDeliveryTime = _waybillItems
                .Where(x => x.Package.Id == deliveredPackageId && x.TypeOfPackage == TypeOfPackages.ToBeDelivered)
                .Select(x=>x.EstimatedDeliveryTime)
                .FirstOrDefault();

            package.CourierRating = CountCouriersRating(package, estimatedDeliveryTime);

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

        private int CountCouriersRating(Package package, DateTime estimatedDeliveryTime)
        {
            var deviation = (package.DeliveryDate - estimatedDeliveryTime).TotalMinutes;

            if (-10 < deviation && deviation < 10)
            {
                return 5;
            }
            if (-20 < deviation && deviation < 20)
            {
                return 4;
            }
            if (-30 < deviation && deviation < 30)
            {
                return 3;
            }
            if (-40 < deviation && deviation < 40)
            {
                return 2;
            }
            else 
            {
                return 1;
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

            var deliveryTime = _startTimeOfWork;

            foreach (var package in _waybill)
            {
                var distance = _locationService.GetDistanceBetweenTwoPlaces(currentLocation, package.Sender.lat, package.Sender.lon);
                var waybillItem = new WaybillItem
                {
                    Package = package,
                    Distance = distance,
                    Time = Math.Round(distance / (double)_vehicle.AverageSpeed * _minutes),
                    TypeOfPackage = TypeOfPackages.ForPickup,
                };
                waybillItem.EstimatedDeliveryTime = deliveryTime.AddMinutes(waybillItem.Time);

                _waybillItems.Add(waybillItem);

                deliveryTime = waybillItem.EstimatedDeliveryTime;
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
                    Time = Math.Round(distance / (double)_vehicle.AverageSpeed * _minutes),
                    TypeOfPackage = TypeOfPackages.ToBeDelivered,
                };
                waybillItem.EstimatedDeliveryTime = deliveryTime.AddMinutes(waybillItem.Time);

                _waybillItems.Add(waybillItem);

                deliveryTime = waybillItem.EstimatedDeliveryTime;
                currentLocation.Lat = package.RecipientLat;
                currentLocation.Lon = package.RecipientLon;
            }

            foreach (var item in _waybillItems)
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
