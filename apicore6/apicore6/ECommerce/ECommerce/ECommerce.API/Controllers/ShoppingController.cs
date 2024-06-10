using ECommerce.API.DataAccess;
using ECommerce.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        readonly IDataAccess dataAccess;
        private readonly string DateFormat;
        public ShoppingController(IDataAccess dataAccess, IConfiguration configuration)
        {
            this.dataAccess = dataAccess;
            DateFormat = configuration["Constants:DateFormat"];
        }

        #region Category
        [HttpGet("GetCategoryList")]
        public IActionResult GetCategoryList()
        {
            var result = dataAccess.GetProductCategories();
            return Ok(result);
        }
        #endregion

        #region Offer

        [HttpGet("GetOfferList")]
        public IActionResult GetOfferlist()
        {
            var result = dataAccess.GetOfferList();
            return Ok(result);
        }

        #endregion

        #region Review

        [HttpPost("InsertReview")]
        public IActionResult InsertReview([FromBody] Review review)
        {
            review.CreatedAt = DateTime.Now.ToString(DateFormat);
            dataAccess.InsertReview(review);
            return Ok("inserted");
        }

        [HttpGet("GetProductReviews/{productId}")]
        public IActionResult GetProductReviews(int productId)
        {
            var result = dataAccess.GetProductReviews(productId);
            return Ok(result);
        }


        #endregion

        #region Cart

        [HttpPost("InsertCartItem/{userid}/{productid}")]
        public IActionResult InsertCartItem(int userid, int productid)
        {
            var result = dataAccess.InsertCartItem(userid, productid);
            return Ok(result ? "inserted" : "not inserted");
        }

        [HttpGet("GetActiveCartOfUser/{id}")]
        public IActionResult GetActiveCartOfUser(int id)
        {
            var result = dataAccess.GetActiveCartOfUser(id);
            return Ok(result);
        }

        [HttpGet("GetAllPreviousCartsOfUser/{id}")]
        public IActionResult GetAllPreviousCartsOfUser(int id)
        {
            var result = dataAccess.GetAllPreviousCartsOfUser(id);
            return Ok(result);
        }

        [HttpDelete("DeleteCartItem/{productId}")]
        public IActionResult DeleteCartItem(int productId)
        {
            // Gọi phương thức xóa `CartItem` dựa trên `ProductId` từ DataAccess
            var result = dataAccess.DeleteCartItemByProductId(productId);

            if (result)
            {
                return Ok("Mục sản phẩm đã được xóa khỏi giỏ hàng.");
            }
            else
            {
                return NotFound("Không tìm thấy mục sản phẩm để xóa.");
            }
        }
        #endregion

        #region PayMent
        [HttpGet("GetPaymentMethods")]
        public IActionResult GetPaymentMethods()
        {
            var result = dataAccess.GetPaymentMethods();
            return Ok(result);
        }

        [HttpPost("InsertPayment")]
        public IActionResult InsertPayment(Payment payment)
        {
            payment.CreatedAt = DateTime.Now.ToString();
            var id = dataAccess.InsertPayment(payment);
            return Ok(id.ToString());
        }

        #endregion

        #region Oder
        [HttpPost("InsertOrder")]
        public IActionResult InsertOrder(Order order)
        {
            order.CreatedAt = DateTime.Now.ToString();
            var id = dataAccess.InsertOrder(order);
            return Ok(id.ToString());
        }

        #endregion

    }
}
