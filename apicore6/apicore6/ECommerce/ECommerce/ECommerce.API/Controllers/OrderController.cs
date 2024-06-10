using ECommerce.API.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        readonly IDataAccess dataAccess;
        private readonly string DateFormat;
        public OrderController(IDataAccess dataAccess, IConfiguration configuration)
        {
            this.dataAccess = dataAccess;
            DateFormat = configuration["Constants:DateFormat"];
        }
        [HttpGet("GetOrder")]
        public ActionResult GetOrder()
        {
            var result = dataAccess.GetOrders();
            return Ok(result);
        }
    }
}
