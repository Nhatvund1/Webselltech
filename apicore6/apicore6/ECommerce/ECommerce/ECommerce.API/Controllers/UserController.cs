using ECommerce.API.DataAccess;
using ECommerce.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly IDataAccess dataAccess;
        private readonly string DateFormat;
        public UserController(IDataAccess dataAccess, IConfiguration configuration)
        {
            this.dataAccess = dataAccess;
            DateFormat = configuration["Constants:DateFormat"];
        }

        [HttpPost("RegisterUser")]
        public IActionResult RegisterUser([FromBody] User user)
        {
            user.CreatedAt = DateTime.Now.ToString(DateFormat);
            user.ModifiedAt = DateTime.Now.ToString(DateFormat);

            var result = dataAccess.InsertUser(user);
            if (result == 1)
            {
                return Ok("Thêm thành công");
            }
            else
            {
                return NotFound("Thêm thất bại");
            }
        }

            [HttpPost("LoginUser")]
        public IActionResult LoginUser([FromBody] User user)
        {
            var token = dataAccess.IsUserPresent(user.Email, user.Password);
            if (token == "") token = "invalid";
            return Ok(token);
        }


        [HttpPut("UpdateUser")]
        public ActionResult UpdateUser(User user)
        {
            user.CreatedAt = DateTime.Now.ToString(DateFormat);
            user.ModifiedAt = DateTime.Now.ToString(DateFormat);
            var result = dataAccess.UpdateUser(user);
            return Ok(result);
        }
    }
}
