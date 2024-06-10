using ECommerce.API.DataAccess;
using ECommerce.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        readonly IDataAccess dataAccess;
        private readonly string DateFormat;
        public ProductController(IDataAccess dataAccess, IConfiguration configuration)
        {
            this.dataAccess = dataAccess;
            DateFormat = configuration["Constants:DateFormat"];
        }
        #region Product


        [HttpGet("GetProducts")]
        public IActionResult GetProducts(string category, string subcategory, int count)
        {
            var result = dataAccess.GetProducts(category, subcategory, count);
            return Ok(result);
        }



        [HttpGet("GetProductsFromQuery")]
        public IActionResult GetProductFromQuery()
        {
            var result = dataAccess.GetProductsFromQuery();
            return Ok(result);
        }



        [HttpGet("GetProduct/{id}")]
        public IActionResult GetProduct(int id)
        {
            var result = dataAccess.GetProduct(id);
            return Ok(result);
        }


        [HttpPost("CreateProduct")]
        public IActionResult CreateProduct(Product product)
        {
            try
            {
                // Gọi hàm InsertProduct(product) hoặc thực hiện thêm sản phẩm vào cơ sở dữ liệu ở đây.
                dataAccess.InsertProduct(product); // Gọi hàm thêm sản phẩm vào cơ sở dữ liệu

                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
                // Trả về HTTP 201 Created và thông tin của sản phẩm đã được tạo thành công.
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi chèn sản phẩm: " + ex.Message);
                // Trả về HTTP 400 Bad Request với thông báo lỗi nếu có lỗi.
            }
        }


        [HttpDelete("DeleteProduct/{productId}")]
        public IActionResult DeleteProduct(int productId)
        {
            var product = dataAccess.DeleteProduct(productId);
            if (product)
            {
                return Ok("Xóa thành công");
            }
            else
            {
                return NotFound("ko tìm thấy id");
            }
        }

        #endregion
    }
}
