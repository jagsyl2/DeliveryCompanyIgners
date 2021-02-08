using DeliveryCompany.DataLayer.Models;
using System;
using System.Collections.Generic;

namespace DeliveryCompany
{
    internal class IoHelperAddPackage
    {
        private IoHelper _ioHelper = new IoHelper();

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

                RecipientName = _ioHelper.GetStringFromUser("Enter the name of the recipient:"),
                RecipientSurname = _ioHelper.GetStringFromUser("Enter the surname of the recipient:"),
                RecipientEmail = _ioHelper.GetEMailFromUser("Enter an e-mail address of recipient:"),
                RecipientStreet = _ioHelper.GetStringFromUser("Enter the street address of the recipient:"),
                RecipientStreetNumber = _ioHelper.GetStringFromUser("Enter the street number of the recipient:"),
                RecipientPostCode = _ioHelper.GetStringFromUser("Enter the post code of the recipient:"),
                RecipientCity = _ioHelper.GetStringFromUser("Enter the city of the recipient:"),

                Size = _ioHelper.GetSizeFromUser("Choose your packege size"),
                DateOfRegistration = DateTime.Now,
                State = StateOfPackage.AwaitingPosting,
            };
            Console.WriteLine($"Number of added package: {newPackage.Number} - state: {newPackage.State}");
            Console.WriteLine();

            return newPackage;
        }
    }
}
