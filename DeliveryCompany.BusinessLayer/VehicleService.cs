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
        Task<Vehicle> GetVehicleAsync(int courierId);
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

        public async Task<Vehicle> GetVehicleAsync(int courierId)
        {
            Vehicle vehicle;

            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                vehicle = await context.Vehicles
                    .FirstOrDefaultAsync(x => x.DriverId == courierId);
            }
            
            return vehicle;
        }
    }
}
