using System.ComponentModel.DataAnnotations;

namespace Layer.Presentation.Models
{
    public class Credential
    {
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(100)]
        [Required]
        public string Password { get; set; }
    }
}
