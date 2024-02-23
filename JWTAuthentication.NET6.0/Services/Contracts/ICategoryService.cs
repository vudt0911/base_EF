using JWTAuthentication.NET6._0.Models.Models;
using JWTAuthentication.NET6._0.Models.DTO;
using JWTAuthentication.NET6._0.Models.Entities;

namespace JWTAuthentication.NET6._0.Services.Contracts
{
    public interface ICategoryService
    {
        public List<CategoryDTO> GetCategories();
        public CategoryDTO? GetCategoryById(int categoryId);
        public CategoryDTO AddCategory(CategoryModel categoryModel);
        public bool UpdateCategory(int categoryId, CategoryModel categoryModel);
        public bool DeleteCategory(int categoryId);
        public void SaveChanges();
        public List<ProductDTO> GetAllProductByCategoryId(int categoryId);
    }
}
