using DeliveryCompany.BusinessLayer;
using DeliveryCompany.BusinessLayer.ScheduledTask;
using System;

namespace DeliveryCompany
{
    class Program
    {
        private DatabaseManagmentService _databaseManagmentService = new DatabaseManagmentService();
        private IoHelperRegisterUser _ioHelperRegisterUser = new IoHelperRegisterUser();
        private IoHelperAddVehicle _ioHelperAddVehicle = new IoHelperAddVehicle();
        private IoHelperAddPackage _ioHelperAddPackage = new IoHelperAddPackage();
        private PackageService _packageService = new PackageService();
        private VehicleService _vehicleService = new VehicleService();
        private UserService _userService = new UserService();
        private IoHelper _ioHelper = new IoHelper();
        private Menu _loginMenu = new Menu();

        private bool _exit;

        static void Main()
        {
            new Program().Run();
        }

        void Run()
        {
            _databaseManagmentService.EnsureDatabaseCreation();
            
            JobScheduler jobScheduler = new JobScheduler();
            jobScheduler.Start();

            Console.WriteLine("Welcome to the application for administering the entire system of the Igners courier company!");

            RegisterMenuOptions();

            do
            {
                _loginMenu.PrintMenuOptions();
                var userChoice = _ioHelper.GetIntFromUser("Choose what you want to do:");
                _loginMenu.GetMenuOption(userChoice);
            } 
            while (!_exit);
        }

        private void RegisterMenuOptions()
        {
            _loginMenu.AddOption(new MenuItem { Key = 1, Action = AddUser, Discription = "Adding a new user." });
            _loginMenu.AddOption(new MenuItem { Key = 2, Action = AddPackage, Discription = "Adding a new package." });
            _loginMenu.AddOption(new MenuItem { Key = 3, Action = AddVehicle, Discription = "Adding a new vehicle." });
            _loginMenu.AddOption(new MenuItem { Key = 4, Action = () => { _exit = true; }, Discription = "Exit." });
        }

        private void AddVehicle()
        {
            var drivers = _userService.GetAllDrivers();
            if (drivers.Count < 1)
            {
                Console.WriteLine();
                _ioHelper.WriteString("It is not possible to add vehicle - minimum 1 driver needed");
                return;
            }

            var driverId = _ioHelperAddVehicle.SelectDriverId(drivers);
            var vehicle = _ioHelperAddVehicle.CreateNewVehicle(driverId);
            
            _vehicleService.Add(vehicle);
        }

        private void AddPackage()
        {
            var customers = _userService.GetAllCustomers();
            if (customers.Count < 1)
            {
                Console.WriteLine();
                _ioHelper.WriteString("It is not possible to add package - minimum 1 customer needed");
                return;
            }

            var customerId = _ioHelperAddPackage.SelectCustomerId(customers);
            var package = _ioHelperAddPackage.CreateNewPackage(customerId);
            
            _packageService.Add(package);
        }

        private void AddUser()
        {
            var user = _ioHelperRegisterUser.CreateNewUser();
            if (user == null)
            {
                return;
            }

            _userService.Add(user);

            Console.WriteLine($"A new {user.Type} has been added.");
        }
    }
}
