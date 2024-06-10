
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Layer.Presentation.Models
{
    public class ReviewModel
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        public string Images { get; set; }
        public int OrderItemId { get; set; }
        public int UserId { get; set; }
    }
}
