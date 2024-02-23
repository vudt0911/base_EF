using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWTAuthentication.NET6._0.Models.Entities
{
    public class CategoryEntity
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; } = string.Empty;
        public ICollection<ProductEntity> ProductEntities { get; } = new List<ProductEntity>(); // Collection navigation containing dependents
    }
}
