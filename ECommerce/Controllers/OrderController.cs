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
        //This API will retrieve all the orders details of the customer with id customerId
        public async Task<IActionResult> GetOrders([FromRoute] Guid customerId)
        {
            var orderList = await orderRepositoryService.GetAllOrders(customerId);
            return orderList.Count > 0 ? Ok(orderList) : NotFound();
        }

        [HttpGet("GetOrder/{orderId}")]
        //This API will return the specific order details of the order with id as orderId
        public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
        {
            var order = await orderRepositoryService.GetOrderById(orderId);
            return order != null ? Ok(order) : NotFound();
        }

        [HttpPost]
        //This API will create a new order 
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto createOrderDto)
        {
            var createOrderResult = await orderRepositoryService.CreateOrder(createOrderDto);
            return createOrderResult.Result ? Ok(createOrderResult) : BadRequest(createOrderResult);
        }

        [HttpPut("Cancel/{orderId}")]
        //This API will change the status of the order to cancelled
        public async Task<IActionResult> CancelOrder([FromRoute] Guid orderId)
        {
            var cancelRequestResult = await orderRepositoryService.CancelOrder(orderId);
            return cancelRequestResult.Result ? Ok(cancelRequestResult) : BadRequest(cancelRequestResult);
        }

        [HttpPut("Return/{orderId}")]
        //This API will change the status of the order to returned
        public async Task<IActionResult> ReturnOrder([FromRoute] Guid orderId)
        {
            var cancelRequestResult = await orderRepositoryService.ReturnOrder(orderId);
            return cancelRequestResult.Result ? Ok(cancelRequestResult) : BadRequest(cancelRequestResult);
        }
    }
}
