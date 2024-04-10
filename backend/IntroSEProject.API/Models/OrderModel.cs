using IntroSEProject.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntroSEProject.API.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        [Required]
        public int Total { get; set; }
        public int SeatNumber { get; set; }
        [StringLength(20)]
        public string Status { get; set; }
        public int UserId { get; set; }
    }
}
