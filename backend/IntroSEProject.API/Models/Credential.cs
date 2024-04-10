using System.ComponentModel.DataAnnotations;

namespace IntroSEProject.API.Models
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
