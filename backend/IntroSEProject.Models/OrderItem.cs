using System.ComponentModel.DataAnnotations;

namespace IntroSEProject.Models
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
