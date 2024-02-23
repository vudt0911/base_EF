namespace JWTAuthentication.NET6._0.Models.DTO
{
    public class GetProductPagingRequest : PagingRequestBase
    {
        public string? Keyword { get; set; }
        public List<int>? CategoryIds { get; set; }
    }
}
