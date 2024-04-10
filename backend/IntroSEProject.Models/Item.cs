using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntroSEProject.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        [StringLength(1000)]
        public string Thumbnail { get; set; }
        [StringLength(100)]
        [Required]
        public string Name { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Stock { get; set; }
        public string? Images { get; set; }
        [ForeignKey(nameof(Category))]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        
    }
}
