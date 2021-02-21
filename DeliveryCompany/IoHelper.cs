using DeliveryCompany.DataLayer.Models;
using System;
using System.Collections.Generic;

namespace DeliveryCompany
{
    public interface IIoHelper
    {
        string GetEMailFromUser(string message);
        int GetIntFromUser(string message);
        PackageSize GetSizeFromUser(string message);
        string GetStringFromUser(string message);
        TypeOfUser GetTypeOfUserFromUser(string massage);
        void PrintUser(User user);
        void PrintUsers(List<User> users, string message);
        void WriteString(string message);
    }

    public class IoHelper : IIoHelper
    {
        public string GetStringFromUser(string message)
        {
            Console.WriteLine($"{message}");
            return Console.ReadLine();
        }

        public int GetIntFromUser(string message)
        {
            int userChoice;

            while (!int.TryParse(GetStringFromUser(message), out userChoice))
            {
                Console.WriteLine("This is not a number. Try again...");
            }

            return userChoice;
        }

        public string GetEMailFromUser(string message)
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

        public void WriteString(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine();
        }

        public TypeOfUser GetTypeOfUserFromUser(string massage)
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

        public PackageSize GetSizeFromUser(string message)
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

        public void PrintUsers(List<User> users, string message)
        {
            Console.WriteLine(message);
            foreach (var user in users)
            {
                PrintUser(user);
            }
        }

        public void PrintUser(User user)
        {
            Console.WriteLine($"{user.Id}.{user.Name} {user.Surname} - {user.Type}");
        }
    }
}