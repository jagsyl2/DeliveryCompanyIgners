using DeliveryCompany.AppForDrivers.Models;
using System;
using System.Collections.Generic;

namespace DeliveryCompany.AppForDrivers
{
    public interface IIoHelper
    {
        int GetIntFromUser(string message);
        string GetStringFromUser(string message);
        void Print(int courierId, List<Rating> ratings);
        void PrintPackageAlongTheRoute(WaybillItem item);
        void PrintPackages(Package package);
    }

    public class IoHelper : IIoHelper
    {
        public void PrintPackages(Package package)
        {
            Console.WriteLine($"Package Id: {package.Id} - Number: {package.Number} - Package size: {package.Size}");
            Console.WriteLine($"Sender Id: {package.SenderId}, Sender: {package.Sender.Name} {package.Sender.Surname}, location: {package.Sender.Lat}, {package.Sender.Lon}");
            Console.WriteLine($"Recipient: {package.RecipientName} {package.RecipientSurname}, location: {package.RecipientLat}, {package.RecipientLon}");
            Console.WriteLine();
        }

        public void PrintPackageAlongTheRoute(WaybillItem item)
        {
            Console.WriteLine($"Package Id: {item.Package.Id} - Number: {item.Package.Number}: - Package {item.TypeOfPackage}");
            Console.WriteLine($"\tDistance from the previous point: {item.Distance}[km]");
            Console.WriteLine($"\tEstimated travel time from previous point: {item.Time}[min]");
            Console.WriteLine($"\tDateTime: {item.EstimatedDeliveryTime}");
            Console.WriteLine();
        }

        public string GetStringFromUser(string message)
        {
            Console.WriteLine($"{message}");
            return Console.ReadLine();
        }

        public int GetIntFromUser(string message)
        {
            int userChoice;

            while (!int.TryParse(GetStringFromUser(message), out userChoice))
            {
                Console.WriteLine("This is not a number. Try again...");
            }

            return userChoice;
        }

        public void Print(int courierId, List<Rating> ratings)
        {
            Console.WriteLine($"Ratings for the courier {courierId}");

            foreach (var rating in ratings)
            {
                Console.WriteLine($"{rating.Id}. Waybill dated: {rating.DateTime.ToString("yyyy.MM.dd")} - rating: {rating.CouriersRating}");
            }
            Console.WriteLine();
        }
    }
}