using DeliveryCompany.BusinessLayer.Distances;
using DeliveryCompany.DataLayer.Models;
using System;

namespace DeliveryCompany
{
    public interface IIoHelperRegisterUser
    {
        User CreateNewUser();
    }

    public class IoHelperRegisterUser : IIoHelperRegisterUser
    {
        private readonly IIoHelper _ioHelper;
        private readonly ILocationService _locationService;

        public IoHelperRegisterUser(
            IIoHelper ioHelper,
            ILocationService locationService)
        {
            _ioHelper = ioHelper;
            _locationService = locationService;
        }

        public User CreateNewUser()
        {
            bool adressExist = false;
            do
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
                    newUser.lat = locationCoordinates.Lat;
                    newUser.lon = locationCoordinates.Lon;
                    
                    return newUser;
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
