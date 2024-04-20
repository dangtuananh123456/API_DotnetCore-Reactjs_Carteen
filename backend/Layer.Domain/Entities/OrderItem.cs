using System.ComponentModel.DataAnnotations;

namespace Layer.Domain.Entities
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int Quantity { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
