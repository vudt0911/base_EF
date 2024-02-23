namespace JWTAuthentication.NET6._0.Models.Models
{
    public class ProductModel
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public int CategoryId { get; set; }
    }
}
