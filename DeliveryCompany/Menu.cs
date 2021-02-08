using System;
using System.Collections.Generic;

namespace DeliveryCompany
{
    internal class Menu
    {
        private Dictionary<int, MenuItem> _option = new Dictionary<int, MenuItem>();

        internal void AddOption(MenuItem item)
        {
            if (_option.ContainsKey(item.Key))
            {
                Console.WriteLine("Oops, unavailable option!");
                return;
            }

            _option.Add(item.Key, item);
        }

        internal void PrintMenuOptions()
        {
            foreach (var item in _option)
            {
                Console.WriteLine($"{item.Key}.{item.Value.Discription}");
            }
        }

        internal void GetMenuOption(int optionNumber)
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
