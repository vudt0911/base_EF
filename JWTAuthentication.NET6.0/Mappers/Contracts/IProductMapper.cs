using JWTAuthentication.NET6._0.Models.DTO;
using JWTAuthentication.NET6._0.Models.Entities;
using JWTAuthentication.NET6._0.Models.Models;

namespace JWTAuthentication.NET6._0.Mappers.Contracts
{
    public interface IProductMapper
    {
        public ProductEntity MapToProduct(ProductModel productModel);
        public ProductDTO MapToProductDTO(ProductEntity productEntity);
    }
}
