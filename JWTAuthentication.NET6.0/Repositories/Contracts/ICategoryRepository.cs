using JWTAuthentication.NET6._0.Data;
using JWTAuthentication.NET6._0.Models.Entities;

namespace JWTAuthentication.NET6._0.Repositories.Contracts
{
    public interface ICategoryRepository : IBaseRepository
    {
        public List<CategoryEntity> getAll();
        public CategoryEntity? GetCategoryById(int id);
        public CategoryEntity AddCategory(CategoryEntity category);
        public bool UpdateCategory(CategoryEntity category);
        public void DeleteCategory(CategoryEntity category);
        public List<ProductEntity> GetAllProductByCategoryId(int id);

    }
}
