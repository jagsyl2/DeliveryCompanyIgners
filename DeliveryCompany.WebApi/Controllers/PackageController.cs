using DeliveryCompany.BusinessLayer;
using DeliveryCompany.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DeliveryCompany.WebApi.Controllers
{
    [Route("api/packages")]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        /*
        Method: POST
        URI: http://localhost:10500/api/packages
        Body: 
            {
              "id": 0,
              "number": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
              "senderId": 7,
              "size": 15,
              "dateOfRegistration": "2021-02-22T19:58:43.778Z",
              "state": 0,
              "vehicleNumber": 0,
              "recipientName": "Sylwia",
              "recipientSurname": "S",
              "recipientEmail": "syla@gmail.com",
              "recipientStreet": "Śliska",
              "recipientStreetNumber": "28D",
              "recipientCity": "Gdynia",
              "recipientPostCode": "81-577",
              "recipientLat": 54.50,
              "recipientLon": 18.5
            }
        */

        [HttpPost]
        public async Task PostPackage([FromBody] Package package)
        {
            try
            {
                await _packageService.Add(package);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
