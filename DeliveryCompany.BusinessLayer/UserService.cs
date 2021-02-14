using DeliveryCompany.DataLayer;
using DeliveryCompany.DataLayer.Models;
using System;
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
        private readonly Func<IDeliveryCompanyDbContext> _deliveryCompanyDbContextFactoryMethod;

        public UserService(Func<IDeliveryCompanyDbContext> deliveryCompanyDbContextFactoryMethod)
        {
            _deliveryCompanyDbContextFactoryMethod = deliveryCompanyDbContextFactoryMethod;
        }

        public void Add(User user)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public void Update(User user)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                context.Users.Update(user);
                context.SaveChanges();
            }
        }

        public List<User> GetAllCustomers()
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return context.Users
                    .AsQueryable()
                    .Where(x => x.Type == TypeOfUser.Customer && (x.lat != 999 || x.lon != 999))
                    .ToList();
            }
        }

        public List<User> GetAllDrivers()
        {
            using(var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return context.Users
                    .AsQueryable()
                    .Where(x => x.Type == TypeOfUser.Driver && (x.lat != 999 || x.lon != 999))
                    .ToList();
            }
        }

        public List<User> GetAllUsersWithoutCoordinates()
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return context.Users
                    .AsQueryable()
                    .Where(x => (x.lat == 999 || x.lon == 999))
                    .ToList();
            }
        }
    }
}