using DeliveryCompany.AppForDrivers.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;

namespace DeliveryCompany.AppForDrivers
{
    class Program
    {
        private readonly IoHelper _ioHelper = new IoHelper();

        private bool _exit = false;

        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            do
            {
                var driver = GetDriver();

                LogIn(driver);
            }
            while (!_exit);
        }

        private void LogIn(User driver)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync($@"http://localhost:10500/api/users/find?email={driver.Email}&password={driver.Password}").Result;
                var responseText = response.Content.ReadAsStringAsync().Result;
                
                if (response.StatusCode==HttpStatusCode.OK)
                {
                    var responseObject = JsonConvert.DeserializeObject<User>(responseText);
                    Menu(responseObject);
                }
                else
                {
                    Console.WriteLine($"Failed. Status code: {response.StatusCode}");
                }
            }
        }

        private void Menu(User user)
        {
            do
            {
                Console.WriteLine("Choose option:");
                Console.WriteLine("1. Get waybill");
                Console.WriteLine("2. Exit");

                var option = _ioHelper.GetIntFromUser("Enter option no:");

                switch (option)
                {
                    case 1:
                        GetWaybill(user);
                        break;
                    case 2:
                        _exit = true;
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            } 
            while (!_exit);
        }

        private void GetWaybill(User user)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync($@"http://localhost:10500/api/packages/waybill/{user.Id}").Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseObject = JsonConvert.DeserializeObject<List<Package>>(responseText);
                    foreach (var package in responseObject)
                    {
                        _ioHelper.PrintPackages(package);
                    }
                }
                else
                {
                    Console.WriteLine($"Failed again. Status code: {response.StatusCode}");
                }
            }
        }

        private User GetDriver()
        {
            Console.WriteLine("Welcome in Igners' Delivery Company!");
            Console.WriteLine("If you want to download the waybill, please log in.");

            var user = new User
            {
                Email = _ioHelper.GetStringFromUser("Enter your e-mail:"),
                Password = _ioHelper.GetStringFromUser("Enter the password:")
            };

            return user;
        }
    }
}
