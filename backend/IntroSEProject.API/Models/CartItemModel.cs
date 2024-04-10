using IntroSEProject.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntroSEProject.API.Models
{
    public class CartItemModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ItemId { get; set; }
        public int UserId { get; set; }
    }
}
