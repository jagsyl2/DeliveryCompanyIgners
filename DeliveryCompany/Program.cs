using DeliveryCompany.BusinessLayer;
using DeliveryCompany.BusinessLayer.SpaceTimeProviders;
using System;
using Unity;

namespace DeliveryCompany
{
    class Program
    {
        private readonly IDatabaseManagmentService  _databaseManagmentService;
        private readonly IIoHelperRegisterUser      _ioHelperRegisterUser;
        private readonly IIoHelperAddVehicle        _ioHelperAddVehicle;
        private readonly IIoHelperAddPackage        _ioHelperAddPackage;
        private readonly IPackageService            _packageService;
        private readonly IVehicleService            _vehicleService;
        private readonly IUserService               _userService;
        private readonly IIoHelper                  _ioHelper;
        private readonly IMenu                      _loginMenu;
        //private readonly ITimeProvider              _fastForwardTimeProvider;
        private readonly ITimerSheduler _timerService;

        //private readonly ITimeProvider _fastForwardTimeProvider = new FastForwardTimeProvider();
        //private TimerSheduler scheduler;
        private bool _exit;
        
        static void Main()
        {
            var container = new UnityDiContainerProvider().GetContainer();

            container.Resolve<Program>().Run();
        }

        public Program (
            IDatabaseManagmentService   databaseManagmentService,
            IIoHelperRegisterUser       ioHelperRegisterUser,
            IIoHelperAddVehicle         ioHelperAddVehicle,
            IIoHelperAddPackage         ioHelperAddPackage,
            IPackageService             packageService,
            IVehicleService             vehicleService,
            IUserService                userService,
            IIoHelper                   ioHelper,
            IMenu                       loginMenu,
            //ITimeProvider               timeProvider,
            ITimerSheduler timerSheduler
            )
        {
            _databaseManagmentService = databaseManagmentService;
            _ioHelperRegisterUser = ioHelperRegisterUser;
            _ioHelperAddVehicle = ioHelperAddVehicle;
            _ioHelperAddPackage = ioHelperAddPackage;
            _packageService = packageService;
            _vehicleService = vehicleService;
            _userService = userService;
            _ioHelper = ioHelper;
            _loginMenu = loginMenu;
            //_fastForwardTimeProvider = timeProvider;
            _timerService = timerSheduler;
        }

        void Run()
        {
            _databaseManagmentService.EnsureDatabaseCreation();
            _databaseManagmentService.UpdatingCoordinatesOfExistingUsersInDatabase();
            _databaseManagmentService.UpdatingCoordinatesOfExistingRecipientsInDatabase();

            _timerService.Start();
            //_timerService.SetTimer();
            //JobScheduler jobScheduler = new JobScheduler();
            //jobScheduler.Start();

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
            
            
            //_loginMenu.AddOption(new MenuItem { Key = 5, Action = WhatTime, Discription = "What time is it?" });
        }

        //private void WhatTime()
        //{
        //    //Console.WriteLine(realTimeProvider.Now);
        //    Console.WriteLine(systemTimeProvider.Now);
        //    Console.WriteLine(DateTime.Now);
        //}

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
            Console.WriteLine();
        }
    }
}
