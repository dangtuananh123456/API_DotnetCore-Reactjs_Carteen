
using System.ComponentModel.DataAnnotations;

namespace Layer.Presentation.Models
{
    public class PaymentModel
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        [StringLength(50)]
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        [StringLength(20)]
        public string Status { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
    }
}
