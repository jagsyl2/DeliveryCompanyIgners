using DeliveryCompany.AppForDrivers.Models;
using System;

namespace DeliveryCompany.AppForDrivers
{
    public class IoHelper
    {
        public void PrintPackages(Package package)
        {
            Console.WriteLine($"Package Id: {package.Id} - Number: {package.Number} - Package size: {package.Size}");
            Console.WriteLine($"Sender Id: {package.SenderId}, Sender: {package.Sender.Name} {package.Sender.Surname}, location: {package.Sender.lat}, {package.Sender.lon}");
            Console.WriteLine($"Recipient: {package.RecipientName} {package.RecipientSurname}, location: {package.RecipientLat}, {package.RecipientLon}");
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
    }
}