
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Layer.Presentation.Models
{
    public class CartItemModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ItemId { get; set; }
        public int UserId { get; set; }
    }
}
