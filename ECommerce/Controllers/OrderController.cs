using ECommerce.Models.DTOs;
using ECommerce.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository orderRepositoryService;

        public OrderController(IOrderRepository orderRepositoryService)
        {
            this.orderRepositoryService = orderRepositoryService;
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetOrders([FromRoute] Guid customerId)
        {
            var orderList = await orderRepositoryService.GetAllOrders(customerId);
            return orderList.Count > 0 ? Ok(orderList) : NotFound();
        }

        [HttpGet("GetOrder/{orderId}")]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
        {
            var order = await orderRepositoryService.GetOrderById(orderId);
            return order != null ? Ok(order) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto createOrderDto)
        {
            var createOrderResult = await orderRepositoryService.CreateOrder(createOrderDto);
            return createOrderResult.Result ? Ok(createOrderResult) : BadRequest(createOrderResult);
        }

        [HttpPut("Cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder([FromRoute] Guid orderId)
        {
            var cancelRequestResult = await orderRepositoryService.CancelOrder(orderId);
            return cancelRequestResult.Result ? Ok(cancelRequestResult) : BadRequest(cancelRequestResult);
        }

        [HttpPut("Return/{orderId}")]
        public async Task<IActionResult> ReturnOrder([FromRoute] Guid orderId)
        {
            var cancelRequestResult = await orderRepositoryService.ReturnOrder(orderId);
            return cancelRequestResult.Result ? Ok(cancelRequestResult) : BadRequest(cancelRequestResult);
        }
    }
}
