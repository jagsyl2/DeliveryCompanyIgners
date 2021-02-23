using DeliveryCompany.BusinessLayer;
using DeliveryCompany.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DeliveryCompany.WebApi.Controllers
{
    [Route("api/vehicles")]
    public class VehicleContrtroller : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleContrtroller(IVehicleService vehicleService)
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
