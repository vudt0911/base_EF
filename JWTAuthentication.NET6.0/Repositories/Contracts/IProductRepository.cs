using JWTAuthentication.NET6._0.Data;
using JWTAuthentication.NET6._0.Models.DTO;
using JWTAuthentication.NET6._0.Models.Entities;

namespace JWTAuthentication.NET6._0.Repositories.Contracts
{
    public interface IProductRepository : IBaseRepository
    {
        List<ProductEntity> GetAll();
        ProductEntity? GetProductById(int productId);
        ProductEntity AddProduct(ProductEntity product);
        bool UpdateProduct(ProductEntity product);
        bool DeleteProduct(ProductEntity product);
        PageResult<ProductDTO> GetAllPage(GetProductPagingRequest request);
    }
}
