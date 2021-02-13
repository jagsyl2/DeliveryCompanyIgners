using System;
using System.Collections.Generic;

namespace DeliveryCompany
{
    public interface IMenu
    {
        void AddOption(MenuItem item);
        void GetMenuOption(int optionNumber);
        void PrintMenuOptions();
    }

    public class Menu : IMenu
    {
        private Dictionary<int, MenuItem> _option = new Dictionary<int, MenuItem>();

        public void AddOption(MenuItem item)
        {
            if (_option.ContainsKey(item.Key))
            {
                Console.WriteLine("Oops, unavailable option!");
                return;
            }

            _option.Add(item.Key, item);
        }

        public void PrintMenuOptions()
        {
            foreach (var item in _option)
            {
                Console.WriteLine($"{item.Key}.{item.Value.Discription}");
            }
        }

        public void GetMenuOption(int optionNumber)
        {
            if (!_option.ContainsKey(optionNumber))
            {
                Console.WriteLine("We don't know what to do. Choose an option from our list.");
                return;
            }

            var option = _option[optionNumber];
            option.Action();
        }
    }
}
