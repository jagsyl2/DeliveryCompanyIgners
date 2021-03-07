using DeliveryCompany.DataLayer;
using DeliveryCompany.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryCompany.BusinessLayer
{
    public interface IVehicleService
    {
        Task AddAsync(Vehicle vehicle);
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

        public async Task AddAsync(Vehicle vehicle)
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                context.Vehicles.Add(vehicle);
                await context.SaveChangesAsync();
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
