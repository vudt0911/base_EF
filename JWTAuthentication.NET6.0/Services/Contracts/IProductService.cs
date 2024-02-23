using JWTAuthentication.NET6._0.Models.DTO;
using JWTAuthentication.NET6._0.Models.Models;

namespace JWTAuthentication.NET6._0.Services.Contracts
{
    public interface IProductService
    {
        List<ProductDTO> GetProducts();
        ProductDTO? GetProductById(int productId);
        ProductDTO AddProduct(ProductModel productModel);
        bool UpdateProduct(int productId, ProductModel productModel);
        bool DeleteProduct(int productId);
        PageResult<ProductDTO> GetAllPage(GetProductPagingRequest request);
    }
}
