using DeliveryCompany.DataLayer.Models;
using System.Collections.Generic;

namespace DeliveryCompany
{
    public interface IIoHelperAddVehicle
    {
        Vehicle CreateNewVehicle(int driverId);
        int SelectDriverId(List<User> drivers);
    }

    public class IoHelperAddVehicle : IIoHelperAddVehicle
    {
        private readonly IIoHelper _ioHelper;

        public IoHelperAddVehicle(IIoHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }

        public int SelectDriverId(List<User> drivers)
        {
            _ioHelper.PrintUsers(drivers, "List of drivers:");

            var driverId = _ioHelper.GetIntFromUser("Select a driver:");

            return driverId;
        }

        public Vehicle CreateNewVehicle(int driverId)
        {
            var newVehicle = new Vehicle
            {
                Mark = _ioHelper.GetStringFromUser("Enter the car make:"),
                Model = _ioHelper.GetStringFromUser("Enter the car model:"),
                RegistrationNumber = _ioHelper.GetStringFromUser("Enter the registration number:"),
                LoadCapacity = _ioHelper.GetIntFromUser("Enter the load capacity of the car [kg]:"),
                AverageSpeed = _ioHelper.GetIntFromUser("Enter average speed of the car [km/h]:"),
                DriverId = driverId,
            };

            return newVehicle;
        }
    }
}
