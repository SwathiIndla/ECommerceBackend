using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EcommerceContext dbContext;

        public OrderRepository(EcommerceContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddOrder(ShippingOrder order)
        {
            await dbContext.ShippingOrders.AddAsync(order);
        }

        public async Task AddOrderedItem(OrderedItem orderedItem)
        {
            await dbContext.OrderedItems.AddAsync(orderedItem);
        }

        public async Task<List<ShippingOrder>> GetAllOrdersOfCustomerDesc(Guid customerId)
        {
            return await dbContext.ShippingOrders.Include(order => order.OrderedItems).ThenInclude(item => item.ProductItem).Where(order => order.CustomerId == customerId).OrderByDescending(order => order.OrderDate).ToListAsync();
        }

        public async Task<ShippingOrder?> GetShippedOrder(Guid orderId)
        {
            return await dbContext.ShippingOrders.Include(order => order.OrderedItems).ThenInclude(item => item.ProductItem).FirstOrDefaultAsync(orders => orders.OrderId == orderId);
        }
    }
}
