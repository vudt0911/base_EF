using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWTAuthentication.NET6._0.Models.Entities
{
    public class ProductEntity
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public int CategoryId { get; set; } // Required foreign key property
        public CategoryEntity CategoryEntity { get; set; } = null!; // Required reference navigation to principal
    }
}
