using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Layer.Presentation.Models
{
    public class SeatReservationModel
    {
        public int Id { get; set; }
        public int SeatNumber { get; set; }
        public int OrderId { get; set; }
        public SeatReservationModel(int Id, int SeatNumber, int OrderId) 
        {
            this.Id = Id;
            this.SeatNumber = SeatNumber;
            this.OrderId = OrderId;
        }
    }
}
