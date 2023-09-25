using AutoMapper;
using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class OrderRepositoryService : IOrderRepository
    {
        private readonly EcommerceContext dbContext;
        private readonly IMapper mapper;

        public OrderRepositoryService(EcommerceContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<OrderResultDto> CancelOrder(Guid orderId)
        {
            var order = await dbContext.ShippingOrders.FirstOrDefaultAsync(orders => orders.OrderId == orderId);
            var result = new OrderResultDto { Result = false, OrderId = orderId};
            if (order != null)
            {
                dbContext.ShippingOrders.Remove(order);
                await dbContext.SaveChangesAsync();
                result.Result = true;
            }
            return result;
        }

        public async Task<OrderResultDto> CreateOrder(CreateOrderRequestDto createOrderDto)
        {
            var cartProductItems = await dbContext.CartProductItems.Include(item => item.ProductItem).Where(item => createOrderDto.CartProductItemIds.Contains(item.CartProductItemId)).ToListAsync();
            OrderResultDto result = new OrderResultDto { Result = false, OrderId = null };
            if(cartProductItems != null && cartProductItems.Count > 0)
            {
                var order = new ShippingOrder
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = createOrderDto.CustomerId,
                    ShippingAddress = createOrderDto.ShippingAddress,
                    OrderDate = DateTime.Now,
                    OrderStatus = "Ordered"
                };
                await dbContext.ShippingOrders.AddAsync(order);
                foreach (var cartProductItem in cartProductItems)
                {
                    var orderItem = new OrderedItem
                    {
                        OrderedItemId = Guid.NewGuid(),
                        ProductItemId = cartProductItem.ProductItemId,
                        OrderId = order.OrderId,
                        Quantity = cartProductItem.Quantity,
                        Price = cartProductItem.ProductItem.Price
                    };
                    await dbContext.OrderedItems.AddAsync(orderItem);
                }
                await dbContext.SaveChangesAsync();
                result.Result = true;
                result.OrderId = order.OrderId;
            }
            return result;
        }

        public async Task<List<OrderDto>> GetAllOrders(Guid customerId)
        {
            var orders = await dbContext.ShippingOrders.Include(order => order.OrderedItems).ThenInclude(item => item.ProductItem).Where(order => order.CustomerId == customerId).ToListAsync();
            var ordersDto = mapper.Map<List<OrderDto>>(orders);
            return ordersDto;
        }

        public async Task<OrderDto> GetOrderById(Guid orderId)
        {
            var order = await dbContext.ShippingOrders.FirstOrDefaultAsync(orders => orders.OrderId == orderId);
            var orderDto = mapper.Map<OrderDto>(order);
            return orderDto;
        }
    }
}
