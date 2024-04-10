using AutoMapper.Configuration.Conventions;
using IntroSEProject.Models;

namespace IntroSEProject.API.Models
{
    public class OrderItemModel
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public OrderItemModel(int id, int itemId, int orderId, int quantity)
        {
            Id =  id;
            ItemId = itemId;
            OrderId = orderId;
            Quantity = quantity;
        }
    }
}
