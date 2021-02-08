using DeliveryCompany.DataLayer.Models;
using System.Collections.Generic;

namespace DeliveryCompany
{
    internal class IoHelperAddVehicle
    {
        private IoHelper _ioHelper = new IoHelper();

        internal int SelectDriverId(List<User> drivers)
        {
            _ioHelper.PrintUsers(drivers, "List of drivers:");

            var driverId = _ioHelper.GetIntFromUser("Select a driver:");

            return driverId;
        }

        internal Vehicle CreateNewVehicle(int driverId)
        {
            var newVehicle = new Vehicle
            {
                Mark = _ioHelper.GetStringFromUser("Enter the car make:"),
                Model = _ioHelper.GetStringFromUser("Enter the car model:"),
                RegistrationNumber = _ioHelper.GetStringFromUser("Enter the registration number:"),
                LoadCapacity = _ioHelper.GetIntFromUser("Enter the load capacity of the car [kg]:"),
                DriverId = driverId,
            };

            return newVehicle;
        }
    }
}
