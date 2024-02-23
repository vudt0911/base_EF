using JWTAuthentication.NET6._0.Auth;
using JWTAuthentication.NET6._0.Data;
using JWTAuthentication.NET6._0.Models.DTO;
using JWTAuthentication.NET6._0.Models.Models;
using JWTAuthentication.NET6._0.Models.DTO;
using JWTAuthentication.NET6._0.Models.Entities;
using JWTAuthentication.NET6._0.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;

namespace JWTAuthentication.NET6._0.Repositories
{
    public class ProductRepository : BaseRepository, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public ProductEntity? AddProduct(ProductEntity product)
        {
            CategoryEntity? categoryEntity = _context.Categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);
            if (categoryEntity == null)
                throw new Exception("Category not found with id: " + product.CategoryId);
            EntityEntry<ProductEntity> entityEntry = _context.Products.Add(product);
            return entityEntry.Entity;
        }

        public bool DeleteProduct(ProductEntity product)
        {
            try {                
                _context.Products.Remove(product);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<ProductEntity> GetAll()
        {
            return _context.Products.ToList();
        }

        public PageResult<ProductDTO> GetAllPage(GetProductPagingRequest request)
        {
            // join
            var query = from p in _context.Products
                        join c in _context.Categories on p.CategoryId equals c.CategoryId
                        select new {p, c};

            // search
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.c.CategoryName.Contains(request.Keyword) || x.p.ProductName.Contains(request.Keyword));

            // filter
            if(request.CategoryIds != null && request.CategoryIds.Count > 0)
                query = query.Where(x => request.CategoryIds.Contains(x.p.CategoryId));

            // order by ProductId in descending order
            query = query.OrderByDescending(x => x.p.ProductId);

            // page
            int totalRow = query.Count();
            var data = query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductDTO()
                {
                    ProductId = x.p.ProductId,
                    ProductName = x.p.ProductName,
                    ProductDescription = x.p.ProductDescription,
                    ProductPrice = x.p.ProductPrice,
                    CategoryId = x.c.CategoryId,
                    CategoryName = x.c.CategoryName,
                }).ToList();

            // select and projection
            var pageResult = new PageResult<ProductDTO>()
            {
                TotalRecord = totalRow,
                Items = data,
                TotalPages = (int)Math.Ceiling((double)totalRow / request.PageSize)
            };

            return pageResult;
        }

        public ProductEntity? GetProductById(int productId)
        {
            return _context.Products.FirstOrDefault(p => p.ProductId == productId);
        }

        public bool UpdateProduct(ProductEntity product)
        {
            ProductEntity? productEntity = GetProductById(product.ProductId);
            if(productEntity == null)
                return false;
            else
                _context.Entry(productEntity).State = EntityState.Detached;
            _context.Attach(product);
            _context.Entry(product).State = EntityState.Modified;
            return true;
        }
    }
}
