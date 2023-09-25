using ECommerce.Models.DTOs;

namespace ECommerce.Repository
{
    public interface IOrderRepository
    {
        Task<List<OrderDto>> GetAllOrders(Guid customerId);
        Task<OrderResultDto> CreateOrder(CreateOrderRequestDto createOrderDto);
        Task<OrderResultDto> CancelOrder(Guid orderId);
        Task<OrderDto> GetOrderById(Guid orderId);
    }
}
