using JWTAuthentication.NET6._0.Mappers.Contracts;
using JWTAuthentication.NET6._0.Models.DTO;
using JWTAuthentication.NET6._0.Models.Models;
using JWTAuthentication.NET6._0.Models.Entities;
using JWTAuthentication.NET6._0.Repositories.Contracts;
using JWTAuthentication.NET6._0.Services.Contracts;

namespace JWTAuthentication.NET6._0.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductMapper _productMapper;
        private readonly IProductRepository _productRepository;
        public ProductService(IProductMapper productMapper, IProductRepository productRepository)
        {
            _productMapper = productMapper;
            _productRepository = productRepository;
        }
        public ProductDTO AddProduct(ProductModel productModel)
        {
            ProductEntity productEntity = _productMapper.MapToProduct(productModel);
            ProductEntity addedProduct = _productRepository.AddProduct(productEntity);
            _productRepository.SaveChanges();
            return _productMapper.MapToProductDTO(addedProduct);
        }

        public bool DeleteProduct(int productId)
        {
            ProductEntity? productEntity = _productRepository.GetProductById(productId);
            if(productEntity == null)
                return false;
            else
            {
                _productRepository.DeleteProduct(productEntity);
                _productRepository.SaveChanges();
                return true;
            }
        }

        public PageResult<ProductDTO> GetAllPage(GetProductPagingRequest request)
        {
            return _productRepository.GetAllPage(request);
        }

        public ProductDTO? GetProductById(int productId)
        {
            ProductEntity? productEntity = _productRepository.GetProductById(productId);
            if(productEntity == null)
                return null;
            else
                return _productMapper.MapToProductDTO(productEntity);
        }

        public List<ProductDTO> GetProducts()
        {
            List<ProductEntity> products = _productRepository.GetAll();
            return products.Select(p => _productMapper.MapToProductDTO(p)).ToList();
        }

        public bool UpdateProduct(int productId, ProductModel productModel)
        {
            productModel.ProductId = productId;
            ProductEntity productEntity = _productMapper.MapToProduct(productModel);
            bool isUpdated = _productRepository.UpdateProduct(productEntity);
            if(isUpdated)
                _productRepository.SaveChanges();
            return isUpdated;
        }
    }
}
