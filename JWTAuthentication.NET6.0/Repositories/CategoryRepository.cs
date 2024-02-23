using JWTAuthentication.NET6._0.Auth;
using JWTAuthentication.NET6._0.Data;
using JWTAuthentication.NET6._0.Mappers;
using JWTAuthentication.NET6._0.Models.Models;
using JWTAuthentication.NET6._0.Models.DTO;
using JWTAuthentication.NET6._0.Models.Entities;
using JWTAuthentication.NET6._0.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace JWTAuthentication.NET6._0.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        private readonly ApplicationDbContext _dataDB;
        public CategoryRepository(ApplicationDbContext dataDB) : base(dataDB)
        {
            _dataDB = dataDB;
        }

        public List<CategoryEntity> getAll()
        {
            return _dataDB.Categories.ToList();
        }

        public CategoryEntity? GetCategoryById(int id)
        {
            return _dataDB.Categories.FirstOrDefault(c => c.CategoryId == id);
        }

        public CategoryEntity AddCategory(CategoryEntity category)
        {
            EntityEntry<CategoryEntity> entityEntry = _dataDB.Categories.Add(category);
            return entityEntry.Entity;
        }

        public bool UpdateCategory(CategoryEntity category)
        {
            CategoryEntity? categoryEntity = GetCategoryById(category.CategoryId);
            if (categoryEntity != null)
            {
                _dataDB.Entry(categoryEntity).State = EntityState.Detached;
                _dataDB.Attach(category);
                _dataDB.Entry(category).State = EntityState.Modified;
                return true;
            }
            return false;
        }

        public void DeleteCategory(CategoryEntity category)
        {
            _dataDB.Categories.Remove(category);
        }

        public List<ProductEntity> GetAllProductByCategoryId(int id)
        {
            return _dataDB.Products.Where(p => p.CategoryId == id).ToList();
        }
    }
}
