using AutoMapper;
using Layer.Domain.Entities;
using Layer.Presentation.Models;

namespace Layer.Presentation.Configs
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<RegisterModel, User>().ReverseMap();
            CreateMap<CategoryModel, Category>().ReverseMap();
            CreateMap<ItemModel, Item>().ReverseMap();
            CreateMap<LoginModel, User>().ReverseMap();
            CreateMap<OrderModel, Order>().ReverseMap();
            CreateMap<SeatReservationModel, SeatReservation>().ReverseMap();
            CreateMap<OrderItemModel, OrderItem>().ReverseMap();
            CreateMap<PaymentModel, Payment>().ReverseMap();
            CreateMap<CartItemModel, CartItem>().ReverseMap();
            CreateMap<ReviewModel, Review>().ReverseMap();
        }
       
    }
}
