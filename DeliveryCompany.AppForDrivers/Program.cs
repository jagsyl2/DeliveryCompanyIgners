using DeliveryCompany.AppForDrivers.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace DeliveryCompany.AppForDrivers
{
    class Program
    {
        private IoHelper _ioHelper = new IoHelper();

        private User driver = null;

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
                var response = httpClient.GetAsync($@"http://localhost:10500/api/users/{driver}").Result;
                var responseText = response.Content.ReadAsStringAsync().Result;
                
                if (response.IsSuccessStatusCode)
                {
                    var responseObject = JsonConvert.DeserializeObject<List<Package>>(responseText);


                }
            }


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
