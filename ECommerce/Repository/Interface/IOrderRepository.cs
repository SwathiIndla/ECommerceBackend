using ECommerce.Models.Domain;

namespace ECommerce.Repository.Interface
{
    public interface IOrderRepository
    {
        Task<ShippingOrder?> GetShippedOrder(Guid orderId);
        Task AddOrder(ShippingOrder order);
        Task AddOrderedItem(OrderedItem orderedItem);
        Task<List<ShippingOrder>> GetAllOrdersOfCustomerDesc(Guid customerId);
    }
}
