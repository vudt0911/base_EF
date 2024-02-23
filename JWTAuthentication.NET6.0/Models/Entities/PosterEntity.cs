using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWTAuthentication.NET6._0.Models.Entities
{
    public class PosterEntity
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PosterId { get; set; }
        public string PosterName { get; set; }
        public string PosterDescription { get; set; }

    }
}
