using JWTAuthentication.NET6._0.Mappers.Contracts;
using JWTAuthentication.NET6._0.Models.Models;
using JWTAuthentication.NET6._0.Models.DTO;
using JWTAuthentication.NET6._0.Models.Entities;

namespace JWTAuthentication.NET6._0.Mappers
{
    public class ProductMapper : IProductMapper
    {
        public ProductEntity MapToProduct(ProductModel productModel)
        {
            ProductEntity produtEntity = new ProductEntity();
            if(productModel.ProductId != null)
                produtEntity.ProductId = (int)productModel.ProductId;
            produtEntity.ProductName = productModel.ProductName;
            produtEntity.ProductDescription = productModel.ProductDescription;
            produtEntity.ProductPrice = productModel.ProductPrice;
            produtEntity.CategoryId = productModel.CategoryId;
            return produtEntity;
        }

        public ProductDTO MapToProductDTO(ProductEntity productEntity)
        {
            ProductDTO productDTO = new ProductDTO();
            productDTO.ProductId = productEntity.ProductId;
            productDTO.ProductName = productEntity.ProductName;
            productDTO.ProductDescription = productEntity.ProductDescription;
            productDTO.ProductPrice = productEntity.ProductPrice;
            productDTO.CategoryId = productEntity.CategoryId;
            return productDTO;
        }
    }
}