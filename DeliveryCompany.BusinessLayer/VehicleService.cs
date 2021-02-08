using DeliveryCompany.DataLayer;
using DeliveryCompany.DataLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryCompany.BusinessLayer
{
    public interface IVehicleService
    {
        public void Add(Vehicle vehicle);
        public List<Vehicle> GetAllVehicles();
        public Vehicle GetVehicle(int courierId);
    }

    public class VehicleService : IVehicleService
    {
        public void Add(Vehicle vehicle)
        {
            using (var context = new DeliveryCompanyDbContext())
            {
                context.Vehicles.Add(vehicle);
                context.SaveChanges();
            }
        }

        public List<Vehicle> GetAllVehicles()
        {
            using (var context = new DeliveryCompanyDbContext())
            {
                return context.Vehicles.ToList();
            }
        }

        public Vehicle GetVehicle(int courierId)
        {
            using(var context = new DeliveryCompanyDbContext())
            {
                return context.Vehicles
                    .FirstOrDefault(x => x.DriverId == courierId); 
            }
        }
    }
}
