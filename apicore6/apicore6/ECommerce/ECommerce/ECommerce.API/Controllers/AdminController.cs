using ECommerce.API.DataAccess;
using ECommerce.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        readonly IDataAccess dataAccess;
        private readonly string DateFormat;
        public AdminController(IDataAccess dataAccess, IConfiguration configuration)
        {
            this.dataAccess = dataAccess;
            DateFormat = configuration["Constants:DateFormat"];
        }
        #region Admin
        [HttpGet("GetAllAdmin")]
        public IActionResult GetAllAdmin()
        {
            var result = dataAccess.GetAllAdmin();
            return Ok(result);
        }
        #endregion

        #region LoginAdmin
        [HttpPost("RegisterAdmin")]
        public IActionResult ReRegisterAdmin([FromBody] Admin admin)
        {
            var result = dataAccess.InsertAdmin(admin);
            string? message;
            if (result) message = "inserted";
            else message = "email not available";
            return Ok(message);

        }
        [HttpPost("LoginAdmin")]
        public IActionResult LoginAdmin([FromBody] Admin admin)
        {
            var token = dataAccess.IsAdminPresent(admin.Email, admin.Password);
            if (token == "") token = "invalid";
            return Ok(token);
        }
        #endregion
    }
}
