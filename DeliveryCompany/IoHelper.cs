using DeliveryCompany.DataLayer.Models;
using System;
using System.Collections.Generic;

namespace DeliveryCompany
{
    internal class IoHelper
    {
        internal string GetStringFromUser(string message)
        {
            Console.WriteLine($"{message}");
            return Console.ReadLine();
        }

        internal int GetIntFromUser(string message)
        {
            int userChoice;

            while (!int.TryParse(GetStringFromUser(message), out userChoice))
            {
                Console.WriteLine("This is not a number. Try again...");
            }

            return userChoice;
        }

        internal string GetEMailFromUser(string message)
        {
            string eMail;
            bool validation;

            do
            {
                eMail = GetStringFromUser(message);
                validation = true;

                if (!eMail.Contains("@"))
                {
                    WriteString("Incorrect adress e-mail (must contain the @ sign). Try again...");

                    validation = false;
                    continue;
                }
            }
            while (validation == false);

            return eMail;
        }

        internal void WriteString(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine();
        }

        internal TypeOfUser GetTypeOfUserFromUser(string massage)
        {
            var correctValues = "";

            foreach (var typeOfUser in (TypeOfUser[])Enum.GetValues(typeof(TypeOfUser)))
            {
                correctValues += $"{typeOfUser},";
            }

            object result;
            while (!Enum.TryParse(typeof(TypeOfUser), GetStringFromUser($"{massage} [{correctValues}]:"), out result))
            {
                Console.WriteLine("We don't know what to do. Please try again and use value from brackets.");
            }

            return (TypeOfUser)result;
        }

        internal PackageSize GetSizeFromUser(string message)
        {
            var correctValues = "";

            foreach (var item in (PackageSize[])Enum.GetValues(typeof(PackageSize)))
            {
                correctValues += $"{item},";
            } 

            object result;

            while (!Enum.TryParse(typeof(PackageSize), GetStringFromUser($"{message} [{correctValues}]:"), out result))
            {
                Console.WriteLine("We don't know what size your packege is. Please try again and use value from brackets.");
            }

            return (PackageSize)result;
        }

        internal void PrintUsers(List<User> users, string message)
        {
            Console.WriteLine(message);
            foreach (var user in users)
            {
                PrintUser(user);
            }
        }

        internal void PrintUser(User user)
        {
            Console.WriteLine($"{user.Id}.{user.Name} {user.Surname} - {user.Type}");
        }
    }
}