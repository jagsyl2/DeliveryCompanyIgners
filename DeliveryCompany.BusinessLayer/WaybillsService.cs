﻿using DeliveryCompany.BusinessLayer.Serializers;
using DeliveryCompany.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeliveryCompany.BusinessLayer
{
    public interface IWaybillsService
    {
        public void CreateWaybills();
    }

    public class WaybillsService : IWaybillsService
    {
        private ILocationService _locationService;
        private IVehicleService _vehicleService;
        private IPackageService _packageService;
        private IJsonSerializer _jsonSerializer;
        private IUserService _userService;

        public WaybillsService(
                ILocationService locationService,
                IVehicleService vehicleService,
                IPackageService packageService,
                IJsonSerializer jsonSerializer,
                IUserService userService)
        {
            _locationService = locationService;
            _vehicleService = vehicleService;
            _packageService = packageService;
            _jsonSerializer = jsonSerializer;
            _userService = userService;
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

                UpdatePackages(todaysPackages);                             //po stworzeniu list przewozowych aktualizuję w bazie danych status na "nadana" oraz dopisuje nr samochodu, którym podrużują
            }
            catch (Exception)
            {                                                               //w przypadku problemów dostaje informację, że nie wygenerował listu przewozowego
                Console.WriteLine();
                Console.WriteLine("\tWaybill info: No waybill generated.");
            }
        }

        private List<Package> PreparingTodaysParcelsForShippin()
        {
            var packages = _packageService.CheckPackagesAwaitingPosting();      //pobieram paczki, których status jest "oczek. na nadanie". w przypadku braku paczek, nie tworzymy listy.
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

        private List<Package> AssignPackagesToCouriers(List<Package> packages, List<User> couriers, Dictionary<int, int> vehiclesLoadCapacity)
        {
            var todaysPackages = new List<Package>();

            foreach (var package in packages)
            {
                var distances = _locationService.CountDistancesFromPackageToCourier(couriers, package);         //każdej paczce, która oczekuje na nadanie obliczam odległości od wszystkich kurierów 
                if (distances.Count == 0)
                {
                    continue;
                }

                Vehicle vehicle;
                do
                {
                    var courierId = FindTheNearestCourier(distances);                                           //wybieram kuriera, który jest najbliżej nadanej paczki

                    vehicle = _vehicleService.GetVehicle(courierId);                                            //jeśli mam kuriera, który jest najbliżej, odnajduję jego samochód
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




                vehiclesLoadCapacity[vehicle.Id] -= (int)package.Size;                                          //jeśli się paczka mieści to zmniejszam dzisiejszą wolną przestrzeń w samochodzie

                package.VehicleNumber = vehicle.Id;                                                             //wszystko jest ok, paczka ma przypisany nr samochodu, którym będzie podróżowała

                todaysPackages.Add(package);                                                                    //tworzę listę paczek, które dzisiaj będą podróżowały
            }

            return todaysPackages;
        }

        private void UpdatePackages(List<Package> packages)
        {
            foreach (var package in packages)
            {
                package.State = StateOfPackage.Given;
                _packageService.Update(package);
            }
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
                var date = DateTime.Now.ToString("yyyy-MM-dd");
                var filePath = Path.Combine(path.FullName, $"{vehicle.DriverId}_{date}.json");

                var waybill = packages
                        .Where(x => x.VehicleNumber == vehicle.Id)
                        .ToList();

                _jsonSerializer.Serialize(filePath, waybill);
            }
        }

        private int FindTheNearestCourier(Dictionary<int, double> distances)
        {
            return distances
                .Where(x => x.Value == distances.Values.Min())
                .Select(x => x.Key)
                .FirstOrDefault();
        }
    }
}
