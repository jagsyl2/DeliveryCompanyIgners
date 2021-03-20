using DeliveryCompany.BusinessLayer;
using DeliveryCompany.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryCompany.WebApiTopShelf.Controllers
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
        public async Task PostPackageAsync([FromBody] Package package)
        {
            await _packageService.AddAsync(package);
        }

        /// <summary>
        /// Enter the vehicle's id
        /// </summary>
        [HttpGet("waybill/{id}")]
        public async Task<List<Package>> GetPackagesOnWaybillAsync(int id)
        {
            return await _packageService.GetPackagesOnCouriersWaybillAsync(id);
        }
        
        [HttpPut("status/{id}")]
        public async Task PutPackageStatusAsync(int id, [FromBody]Package package)
        {
            await _packageService.UpdateByIdAsync(id, package);
        }

        [HttpPut("waybill")]
        public async Task PutPackagesOnWaybillAsync([FromBody]Package package)
        {
            await _packageService.UpdateAsync(package);
        }
    }
}
