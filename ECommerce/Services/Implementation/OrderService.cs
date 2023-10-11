using AutoMapper;
using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository.Interface;
using ECommerce.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IMapper mapper;
        private readonly IOrderRepository orderRepository;
        private readonly IProductRepository productRepository;
        private readonly ISaveChangesRepository saveChangesRepository;
        private readonly ICartRepository cartRepository;

        public OrderService(IMapper mapper, IOrderRepository orderRepository, IProductRepository productRepository, ISaveChangesRepository saveChangesRepository,
             ICartRepository cartRepository)
        {
            this.mapper = mapper;
            this.orderRepository = orderRepository;
            this.productRepository = productRepository;
            this.saveChangesRepository = saveChangesRepository;
            this.cartRepository = cartRepository;
        }

        public async Task<OrderResultDto> CancelOrder(Guid orderId)
        {
            var order = await orderRepository.GetShippedOrder(orderId);
            var result = new OrderResultDto { Result = false, OrderId = orderId };
            if (order != null)
            {
                order.OrderStatus = "Cancelled";
                foreach (var orderItem in order.OrderedItems)
                {
                    var productItem = await productRepository.GetProductItemById(orderItem.ProductItemId);
                    if (productItem != null)
                    {
                        productItem.QtyInStock += orderItem.Quantity;
                    }
                }
                await saveChangesRepository.AsynchronousSaveChanges();
                result.Result = true;
            }
            return result;
        }

        public async Task<OrderResultDto> ReturnOrder(Guid orderId)
        {
            var order = await orderRepository.GetShippedOrder(orderId);
            var result = new OrderResultDto { Result = false, OrderId = orderId };
            if (order != null)
            {
                order.OrderStatus = "Returned";
                foreach (var orderItem in order.OrderedItems)
                {
                    var productItem = await productRepository.GetProductItemById(orderItem.ProductItemId);
                    if (productItem != null)
                    {
                        productItem.QtyInStock += orderItem.Quantity;
                    }
                }
                await saveChangesRepository.AsynchronousSaveChanges();
                result.Result = true;
            }
            return result;
        }

        public async Task<OrderResultDto> CreateOrder(CreateOrderRequestDto createOrderDto)
        {
            var cartProductItems = await cartRepository.GetCartProductItemsList(createOrderDto);
            var result = new OrderResultDto { Result = false, OrderId = null };
            if (cartProductItems != null && cartProductItems.Count > 0)
            {
                var order = new ShippingOrder
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = createOrderDto.CustomerId,
                    ShippingAddress = createOrderDto.ShippingAddress,
                    OrderDate = DateTime.Now,
                    OrderStatus = "Ordered"
                };
                await orderRepository.AddOrder(order);
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
                    var productItem = await productRepository.GetProductItemById(cartProductItem.ProductItemId);
                    if (productItem != null)
                    {
                        productItem.QtyInStock -= cartProductItem.Quantity;
                    }
                    await orderRepository.AddOrderedItem(orderItem);
                    cartRepository.RemoveProductItemFromCart(cartProductItem);
                }
                await saveChangesRepository.AsynchronousSaveChanges();
                result.Result = true;
                result.OrderId = order.OrderId;
            }
            return result;
        }

        public async Task<List<OrderDto>> GetAllOrders(Guid customerId)
        {
            var orders = await orderRepository.GetAllOrdersOfCustomerDesc(customerId);
            var ordersDto = mapper.Map<List<OrderDto>>(orders);
            return ordersDto;
        }

        public async Task<OrderDto> GetOrderById(Guid orderId)
        {
            var order = await orderRepository.GetShippedOrder(orderId);
            var orderDto = mapper.Map<OrderDto>(order);
            return orderDto;
        }
    }
}
