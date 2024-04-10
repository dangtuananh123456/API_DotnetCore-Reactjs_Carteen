using System.ComponentModel.DataAnnotations;

namespace IntroSEProject.API.Models
{
    public class RegisterModel
    {
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(100)]
        [Required]
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
        [StringLength(100)]
        [Required]
        public string? FullName { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Avatar { get; set; }
        //public string EmailConfirmSuccessPage { get; set; }
    }
}
