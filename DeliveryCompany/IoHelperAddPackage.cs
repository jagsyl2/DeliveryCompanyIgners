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

                Recipient = new Recipient
                {
                    Name = _ioHelper.GetStringFromUser("Enter the name of the recipient:"),
                    Surname = _ioHelper.GetStringFromUser("Enter the surname of the recipient:"),
                    Email = _ioHelper.GetEMailFromUser("Enter an e-mail address of recipient:"),
                    Street = _ioHelper.GetStringFromUser("Enter the street address of the recipient:"),
                    StreetNumber = _ioHelper.GetStringFromUser("Enter the street number of the recipient:"),
                    PostCode = _ioHelper.GetStringFromUser("Enter the post code of the recipient:"),
                    City = _ioHelper.GetStringFromUser("Enter the city of the recipient:"),
                },
            };

            try
            {
                var locationCoordinates = _locationService.ChangeLocationToCoordinates(newPackage.Recipient);
                newPackage.Recipient.Lat = locationCoordinates.Lat;
                newPackage.Recipient.Lon = locationCoordinates.Lon;
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
