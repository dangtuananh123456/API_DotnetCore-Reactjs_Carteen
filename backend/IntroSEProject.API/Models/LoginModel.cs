using System.ComponentModel.DataAnnotations;

namespace IntroSEProject.API.Models
{
    public class LoginModel
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        [StringLength(100)]
        [Required]
        public string? FullName { get; set; }
        [StringLength(1000)]
        public string? Avatar { get; set; }
        [StringLength(10)]
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [StringLength(20)]
        public string Role { get; set; }
    }
}
