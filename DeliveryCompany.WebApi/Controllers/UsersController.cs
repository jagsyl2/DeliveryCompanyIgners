using DeliveryCompany.BusinessLayer;
using DeliveryCompany.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DeliveryCompany.WebApi.Controllers
{
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /*
        Method: POST
        URI: http://localhost:10500/api/users
        Body: 
            {
              "id": 0,
              "name": "Antek",
              "surname": "D",
              "email": "antek@gmail.com",
              "street": "JagielLońska",
              "streetNumber": "29",
              "postCode": "34-500",
              "city": "Zakopane",
              "lat": 49.299444,
              "lon": 19.951944,
              "type": 0
            }
        */
        [HttpPost]
        public async Task AddUserAsync([FromBody] User user)
        {
            await _userService.AddAsync(user);
        }

        [HttpGet]
        public User GetUser([FromBody] User user)
        {
            return _userService.GetDriver(user);
        }
    }
}
