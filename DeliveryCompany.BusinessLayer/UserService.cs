using DeliveryCompany.DataLayer;
using DeliveryCompany.DataLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryCompany.BusinessLayer
{
    public interface IUserService
    {
        public void Add(User user);
        public List<User> GetAllCustomers();
        public List<User> GetAllDrivers();
    }

    public class UserService : IUserService
    {
        public void Add(User user)
        {
            using (var context = new DeliveryCompanyDbContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public List<User> GetAllCustomers()
        {
            using (var context = new DeliveryCompanyDbContext())
            {
                return context.Users
                    .Where(x => x.Type == TypeOfUser.Customer)
                    .ToList();
            }
        }

        public List<User> GetAllDrivers()
        {
            using(var context = new DeliveryCompanyDbContext())
            {
                return context.Users
                    .Where(x => x.Type == TypeOfUser.Driver)
                    .ToList();
            }
        }
    }
}