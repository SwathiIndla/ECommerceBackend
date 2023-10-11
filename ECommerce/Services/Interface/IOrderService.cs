using ECommerce.Models.DTOs;

namespace ECommerce.Services.Interface
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllOrders(Guid customerId);
        Task<OrderResultDto> CreateOrder(CreateOrderRequestDto createOrderDto);
        Task<OrderResultDto> CancelOrder(Guid orderId);
        Task<OrderResultDto> ReturnOrder(Guid orderId);
        Task<OrderDto> GetOrderById(Guid orderId);
    }
}
