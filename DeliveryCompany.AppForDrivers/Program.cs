using DeliveryCompany.AppForDrivers.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System.IO;

namespace DeliveryCompany.AppForDrivers
{
    class Program
    {
        private IoHelper _ioHelper = new IoHelper();

        //private User driver = null;

        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            LogIn();
        }

        private void LogIn()
        {
            var driver = GetDriverFromUser();

            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync($@"http://localhost:10500/api/users/find?email={driver.Email}&password={driver.Password}").Result;
                var responseText = response.Content.ReadAsStringAsync().Result;
                
                if (response.StatusCode==HttpStatusCode.OK)
                {
                    var reponseObject = JsonConvert.DeserializeObject<User>(responseText);
                    //var date = _fastForwardTimeProvider.Now.ToString("yyyy-MM-dd");
                    //var filePath = Path.Combine(path.FullName, $"{vehicle.DriverId}_{date}.json");

                    var response2 = httpClient.GetAsync($@"http://localhost:10500/api/packages/{reponseObject.Id}").Result;
                    var responseText2 = response2.Content.ReadAsStringAsync().Result;
                    if (response2.StatusCode == HttpStatusCode.OK)
                    {
                        var responseObject = JsonConvert.DeserializeObject<List<Package>>(responseText2);
                        foreach (var package in responseObject)
                        {
                            PrintPackages(package);
                        }
                        Console.WriteLine($"Success. Response content: ");
                        //PrintDriver(responseObject);
                    }
                    else
                    {
                        Console.WriteLine($"Failed again. Status code: {response.StatusCode}");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed. Status code: {response.StatusCode}");
                }
            }
        }

        private void PrintPackages(Package package)
        {
            Console.WriteLine($"Name: {package.Number}, Breed: {package.RecipientCity}");
        }

        private void PrintDriver(User driver)
        {
            Console.WriteLine($"Name: {driver.Name}, Breed: {driver.Id}");
        }
    

        private User GetDriverFromUser()
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
