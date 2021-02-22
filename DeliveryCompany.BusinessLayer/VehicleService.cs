using DeliveryCompany.DataLayer;
using DeliveryCompany.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryCompany.BusinessLayer
{
    public interface IVehicleService
    {
        void Add(Vehicle vehicle);
        List<Vehicle> GetAllVehicles();
        Vehicle GetVehicle(int courierId);
    }

    public class VehicleService : IVehicleService
    {
        private readonly Func<IDeliveryCompanyDbContext> _deliveryCompanyDbContextFactoryMethod;

        public VehicleService(Func<IDeliveryCompanyDbContext> deliveryCompanyDbContextFactoryMethod)
        {
            _deliveryCompanyDbContextFactoryMethod = deliveryCompanyDbContextFactoryMethod;
        }

        public void Add(Vehicle vehicle)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                context.Vehicles.Add(vehicle);
                context.SaveChanges();
            }
        }

        public List<Vehicle> GetAllVehicles()
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return context.Vehicles.ToList();
            }
        }

        public Vehicle GetVehicle(int courierId)
        {
            using(var context = _deliveryCompanyDbContextFactoryMethod())
            {
                return context.Vehicles
                    .FirstOrDefault(x => x.DriverId == courierId); 
            }
        }
    }
}
