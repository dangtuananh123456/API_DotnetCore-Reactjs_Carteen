
using System.ComponentModel.DataAnnotations;

namespace IntroSEProject.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(100)]
        [Required]
        public string Password { get; set; }
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
        public string Role { get; set; } = "Customer";
        public bool EmailConfirmed { get; set; } = false;
        public string? EmailConfirmToken { get; set; }
        //public virtual ICollection<CartItem> CartItems { get; set; }
        //public virtual ICollection<Review> Reviews { get; set; }
        //public virtual ICollection<Order> Orders { get; set; }
        //public virtual ICollection<Payment> Payments { get; set; }
    }

}