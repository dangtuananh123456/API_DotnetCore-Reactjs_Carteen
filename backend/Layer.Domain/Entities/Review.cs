using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Layer.Domain.Entities
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public int Rating { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        public string Images { get; set; }
        [ForeignKey(nameof(OrderItem))]
        public int OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
