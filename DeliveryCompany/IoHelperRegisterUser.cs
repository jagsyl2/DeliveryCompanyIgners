using DeliveryCompany.BusinessLayer;
using DeliveryCompany.DataLayer.Models;
using System;

namespace DeliveryCompany
{
    internal class IoHelperRegisterUser
    {
        private IoHelper _ioHelper = new IoHelper();
        private LocationService _locationService = new LocationService();

        internal User CreateNewUser()
        {
            var newUser = new User()
            {
                Name = _ioHelper.GetStringFromUser("What's your name?"),
                Surname = _ioHelper.GetStringFromUser("What's your surname?"),

                Street = _ioHelper.GetStringFromUser("Enter the street address:"),
                StreetNumber = _ioHelper.GetStringFromUser("Enter the street number:"),
                PostCode = _ioHelper.GetStringFromUser("Enter the post code"),
                City = _ioHelper.GetStringFromUser("Enter the city"),

                Email = _ioHelper.GetEMailFromUser("What's your e-mail address?"),
                Type = _ioHelper.GetTypeOfUserFromUser("Are you registering as a "),
            };

            try
            {
                var locationCoordinates = _locationService.ChangeLocationToCoordinates(newUser);
                newUser.lat = locationCoordinates[0].lat;
                newUser.lon = locationCoordinates[0].lon;
            }
            catch (Exception)
            {
                Console.WriteLine("The given address does not exist. Try again...");
                return null;
            }

            return newUser;
        }
    }
}
