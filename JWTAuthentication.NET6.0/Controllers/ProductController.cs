using JWTAuthentication.NET6._0.Models.DTO;
using JWTAuthentication.NET6._0.Models.Models;
using JWTAuthentication.NET6._0.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthentication.NET6._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public ActionResult GetProducts()
        {
            return Ok(_productService.GetProducts());
        }

        [HttpGet("{productId}")]
        public IActionResult GetProductById([FromRoute] int productId)
        {
            var product = _productService.GetProductById(productId);
            return product != null ? Ok(product) : NotFound();
        }

        [HttpPost]
        public IActionResult AddProduct([FromBody] ProductModel productModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    var addedProduct = _productService.AddProduct(productModel);
                    return CreatedAtAction(nameof(GetProductById), new { productId = addedProduct.ProductId }, addedProduct);
                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
                }
            }
        }

        [HttpPut("{productId}")]
        public IActionResult UpdateProduct([FromRoute] int productId, [FromBody] ProductModel productModel)
        {
            bool updated = _productService.UpdateProduct(productId, productModel);
            if (updated)
                return Ok("update product success !");
            else
                return new ObjectResult("update product failed !") { StatusCode = StatusCodes.Status500InternalServerError };
        }

        [HttpDelete("{productId}")]
        public IActionResult DeleteProduct([FromRoute] int productId)
        {
            bool deleted = _productService.DeleteProduct(productId);
            if (deleted)
                return Ok("delete product success !");
            else
                return new ObjectResult("delete product failed !") { StatusCode = StatusCodes.Status500InternalServerError };
        }

        [HttpGet]
        [Route("getAllPage")]
        public IActionResult GetAllPage([FromQuery] GetProductPagingRequest request)
        {
            PageResult<ProductDTO> pageResult = _productService.GetAllPage(request);
            return Ok(pageResult);
        }
    }
}
