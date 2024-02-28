using System.ComponentModel.DataAnnotations;

namespace JWTAuthentication.NET6._0.Models.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? ClientURI { get; set; }
    }
}
