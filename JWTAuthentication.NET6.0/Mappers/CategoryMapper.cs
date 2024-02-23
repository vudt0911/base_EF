using JWTAuthentication.NET6._0.Mappers.Contracts;
using JWTAuthentication.NET6._0.Models.Models;
using JWTAuthentication.NET6._0.Models.DTO;
using JWTAuthentication.NET6._0.Models.Entities;

namespace JWTAuthentication.NET6._0.Mappers
{
    public class CategoryMapper : ICategoryMapper
    {
        public CategoryEntity MapToCategory(CategoryModel categoryModel)
        {
            CategoryEntity categoryEntity = new CategoryEntity();
            if (categoryModel.CategoryId != null)
                categoryEntity.CategoryId = (int)categoryModel.CategoryId;
            
            categoryEntity.CategoryName = categoryModel.CategoryName;
            categoryEntity.CategoryDescription = categoryModel.CategoryDescription;
            return categoryEntity;
        }

        public CategoryDTO MapToCategoryDTO(CategoryEntity category)
        {
            CategoryDTO categoryDTO = new CategoryDTO();
            categoryDTO.CategoryId = category.CategoryId;
            categoryDTO.CategoryName = category.CategoryName;
            categoryDTO.CategoryDescription = category.CategoryDescription;
            return categoryDTO;
        }
    }
}