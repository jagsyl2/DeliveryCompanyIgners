using DeliveryCompany.DataLayer.Models;

namespace DeliveryCompany
{
    public interface IIoHelperRegisterUser
    {
        User CreateNewUser();
    }

    public class IoHelperRegisterUser : IIoHelperRegisterUser
    {
        private readonly IIoHelper _ioHelper;

        public IoHelperRegisterUser(IIoHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }

        public User CreateNewUser()
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
                Password = _ioHelper.GetPasswordFromUser("Enter the password:"),
            };

            return newUser;
        }
    }
}
