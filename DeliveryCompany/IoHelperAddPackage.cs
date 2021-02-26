using DeliveryCompany.DataLayer.Models;
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

        public IoHelperAddPackage(IIoHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }

        public int SelectCustomerId(List<User> customers)
        {
            _ioHelper.PrintUsers(customers, "List of customers:");

            var customerId = _ioHelper.GetIntFromUser("Select a customer:");

            return customerId;
        }

        public Package CreateNewPackage(int customerId)
        {
            var newPackage = new Package
            {
                SenderId = customerId,
                Size = _ioHelper.GetSizeFromUser("Choose your packege size"),

                RecipientName = _ioHelper.GetStringFromUser("Enter the name of the recipient:"),
                RecipientSurname = _ioHelper.GetStringFromUser("Enter the surname of the recipient:"),
                RecipientEmail = _ioHelper.GetEMailFromUser("Enter an e-mail address of recipient:"),
                RecipientStreet = _ioHelper.GetStringFromUser("Enter the street address of the recipient:"),
                RecipientStreetNumber = _ioHelper.GetStringFromUser("Enter the street number of the recipient:"),
                RecipientPostCode = _ioHelper.GetStringFromUser("Enter the post code of the recipient:"),
                RecipientCity = _ioHelper.GetStringFromUser("Enter the city of the recipient:"),
            };

            return newPackage;
        }
    }
}
