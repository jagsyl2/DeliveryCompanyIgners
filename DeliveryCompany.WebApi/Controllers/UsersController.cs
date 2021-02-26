using DeliveryCompany.BusinessLayer;
using DeliveryCompany.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DeliveryCompany.WebApiTopShelf.Controllers
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

        /*
        Method: POST
        URI: http://localhost:10500/api/users/find?email={email}&password={password}
        Body: 
        */
        [HttpGet("find")]
        public async Task<User> GetUserAsync([FromQuery] string email, [FromQuery] string password)
        {
            return await _userService.GetDriverAsync(email, password);
        }
    }
}
