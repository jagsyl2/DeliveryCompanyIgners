using DeliveryCompany.BusinessLayer;
using DeliveryCompany.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DeliveryCompany.WebApiTopShelf.Controllers
{
    [Route("api/vehicles")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        /*
        Method: POST
        URI: http://localhost:10500/api/vehicles
        Body: 
            {
              "id": 0,
              "mark": "Mercedes",
              "model": "GLA",
              "registrationNumber": "BA56421",
              "loadCapacity": 400,
              "averageSpeed": 110,
              "driverId": 6
            }
        */
        [HttpPost]
        public async Task PostVehicleAsync([FromBody] Vehicle vehicle)
        {
            await _vehicleService.AddAsync(vehicle);
        }
    }
}
