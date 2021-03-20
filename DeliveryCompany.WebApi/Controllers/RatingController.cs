using DeliveryCompany.BusinessLayer;
using DeliveryCompany.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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

        [HttpGet ("rating/{id}")]
        public List<Rating> GetRatings(int id)
        {
            return _courierRatingsService.GetListOfRatings(id);
        }
    }
}
