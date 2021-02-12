using DeliveryCompany.BusinessLayer.Distances;
using DeliveryCompany.DataLayer.Models;
using System;
using System.Collections.Generic;

namespace DeliveryCompany
{
    internal class IoHelperAddPackage
    {
        private IoHelper _ioHelper = new IoHelper();
        private LocationService _locationService = new LocationService();

        internal int SelectCustomerId(List<User> customers)
        {
            _ioHelper.PrintUsers(customers, "List of customers:");

            var customerId = _ioHelper.GetIntFromUser("Select a customer:");

            return customerId;
        }

        internal Package CreateNewPackage(int customerId)
        {
            var newPackage = new Package
            {
                Number = Guid.NewGuid(),
                SenderId = customerId,

                Size = _ioHelper.GetSizeFromUser("Choose your packege size"),
                DateOfRegistration = DateTime.Now,
                State = StateOfPackage.AwaitingPosting,

                RecipientName = _ioHelper.GetStringFromUser("Enter the name of the recipient:"),
                RecipientSurname = _ioHelper.GetStringFromUser("Enter the surname of the recipient:"),
                RecipientEmail = _ioHelper.GetEMailFromUser("Enter an e-mail address of recipient:"),
                RecipientStreet = _ioHelper.GetStringFromUser("Enter the street address of the recipient:"),
                RecipientStreetNumber = _ioHelper.GetStringFromUser("Enter the street number of the recipient:"),
                RecipientPostCode = _ioHelper.GetStringFromUser("Enter the post code of the recipient:"),
                RecipientCity = _ioHelper.GetStringFromUser("Enter the city of the recipient:"),
            };

            try
            {
                var locationCoordinates = _locationService.ChangeLocationToCoordinates(newPackage.RecipientCity, newPackage.RecipientPostCode, newPackage.RecipientStreet, newPackage.RecipientStreetNumber);
                newPackage.RecipientLat = locationCoordinates.Lat;
                newPackage.RecipientLon = locationCoordinates.Lon;
            }
            catch (Exception)
            {
                Console.WriteLine("The given address does not exist. Try again...");
                return null;
            }

            Console.WriteLine($"Number of added package: {newPackage.Number} - state: {newPackage.State}");
            Console.WriteLine();

            return newPackage;
        }
    }
}
