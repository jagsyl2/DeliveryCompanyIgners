using Microsoft.AspNetCore.Mvc;

namespace DeliveryCompany.WebApi.Controllers
{
    [Route("api/status")]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public string GetStatus()
        {
            return "Status OK";
        }
    }
}
