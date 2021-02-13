using DeliveryCompany.BusinessLayer.Distances;
using DeliveryCompany.DataLayer.Models;
using System;
using System.Collections.Generic;

namespace DeliveryCompany
{
    public interface IIoHelperAddPackage
    {
        Package CreateNewPackage(int customerId);
        int SelectCustomerId(List<User> customers);
    }

    public class IoHelperAddPackage : IIoHelperAddPackage
    {
        private readonly IIoHelper _ioHelper;
        private readonly ILocationService _locationService;

        public IoHelperAddPackage(
            IIoHelper ioHelper,
            ILocationService locationService)
        {
            _ioHelper = ioHelper;
            _locationService = locationService;
        }

        public int SelectCustomerId(List<User> customers)
        {
            _ioHelper.PrintUsers(customers, "List of customers:");

            var customerId = _ioHelper.GetIntFromUser("Select a customer:");

            return customerId;
        }

        public Package CreateNewPackage(int customerId)
        {
            bool adressExist = false;

            do
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

                    Console.WriteLine($"Number of added package: {newPackage.Number} - state: {newPackage.State}");
                    Console.WriteLine();

                    return newPackage;
                }
                catch (Exception)
                {
                    Console.WriteLine("The given address does not exist. Try again...");
                    adressExist = false;
                }
            } 
            while (adressExist == false);
            
            return null;
        }
    }
}
