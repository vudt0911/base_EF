using JWTAuthentication.NET6._0.Models.DTO;
using JWTAuthentication.NET6._0.Models.Models;
using JWTAuthentication.NET6._0.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthentication.NET6._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public ActionResult GetCategories()
        {
            List<CategoryDTO> categories = _categoryService.GetCategories();
            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        public IActionResult GetCategoryById([FromRoute] int categoryId)
        {
            CategoryDTO? category = _categoryService.GetCategoryById(categoryId);
            return category != null ? Ok(category) : NotFound();
        }

        [HttpPost]
        public IActionResult AddCategory([FromBody] CategoryModel categoryModel)
        {
           if(!ModelState.IsValid)
            {
               return BadRequest(ModelState);
           }
           else
           {
                try
                {
                    CategoryDTO addedCategory = _categoryService.AddCategory(categoryModel);
                    return CreatedAtAction(nameof(GetCategoryById), new { categoryId = addedCategory.CategoryId }, addedCategory);
                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
                }
           }
        }

        [HttpPut("{categoryId}")]
        public IActionResult UpdateCategory([FromRoute] int categoryId, [FromBody] CategoryModel categoryModel)
        {
            categoryModel.CategoryId = categoryId;
            bool updated = _categoryService.UpdateCategory(categoryId, categoryModel);
            if (updated)
            {
               _categoryService.SaveChanges();
                return Ok(categoryModel);
            }
            return NotFound("Category Not Found!!!");
        }

        [HttpDelete("{categoryId}")]
        public IActionResult DeleteCategory(int categoryId)
        {
            bool deleted = _categoryService.DeleteCategory(categoryId);
            if (deleted)
            {
                _categoryService.SaveChanges();
                return Ok("Category Delete Successfully!!!");
            }
            return NotFound("Category Not Found!!!");
        }

        [HttpGet("{categoryId}/products")]
        public IActionResult GetAllProductByCategoryId(int categoryId)
        {
            List<ProductDTO> products = _categoryService.GetAllProductByCategoryId(categoryId);
            return Ok(products);
        }
    }
}
