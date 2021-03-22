using DeliveryCompany.BusinessLayer;
using DeliveryCompany.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryCompany.WebApiTopShelf.Controllers
{
    [Route("api/waybills")]
    public class RatingController : ControllerBase
    {
        private readonly ICourierRatingsService _courierRatingsService;

        public RatingController(ICourierRatingsService courierRatingsService)
        {
            _courierRatingsService = courierRatingsService;
        }

        [HttpGet("rating/{id}")]
        public async Task<List<Rating>> GetRatingsAsync(int id)
        {
            return await _courierRatingsService.GetListOfRatingsAsync(id);
        }
    }
}
