using DeliveryCompany.DataLayer.Models;

namespace DeliveryCompany
{
    internal class IoHelperRegisterUser
    {
        private IoHelper _ioHelper = new IoHelper();

        internal User CreateNewUser()
        {
            var newUser = new User()
            {
                Name = _ioHelper.GetStringFromUser("What's your name?"),
                Surname = _ioHelper.GetStringFromUser("What's your surname?"),
                Email = _ioHelper.GetEMailFromUser("What's your e-mail address?"),

                Street = _ioHelper.GetStringFromUser("Enter the street address:"),
                StreetNumber = _ioHelper.GetStringFromUser("Enter the street number:"),
                PostCode = _ioHelper.GetStringFromUser("Enter the post code"),
                City = _ioHelper.GetStringFromUser("Enter the city"),

                Type = _ioHelper.GetTypeOfUserFromUser("Are you registering as a "),
            };

            return newUser;
        }
    }
}
