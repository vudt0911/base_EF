namespace JWTAuthentication.NET6._0.Models.DTO
{
    public class PageResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalRecord { get; set; }
        public int TotalPages { get; set; }
    }
}
