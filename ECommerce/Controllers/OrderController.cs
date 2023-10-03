using ECommerce.Models.DTOs;
using ECommerce.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    /// <summary>
    /// This API handles all the logic related to Orders
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository orderRepositoryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderRepositoryService"></param>
        public OrderController(IOrderRepository orderRepositoryService)
        {
            this.orderRepositoryService = orderRepositoryService;
        }

        /// <summary>
        /// Retrieves all the order information of the customer
        /// </summary>
        /// <param name="customerId">Guid</param>
        /// <returns>Returns 200Ok response with List(OrderDto) if orders are found otherwise returns 404NotFound</returns>
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetOrders([FromRoute] Guid customerId)
        {
            try
            {
                var orderList = await orderRepositoryService.GetAllOrders(customerId);
                return orderList.Count > 0 ? Ok(orderList) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the information of the specific order of the customer
        /// </summary>
        /// <param name="orderId">Guid</param>
        /// <returns>Returns 200Ok response with OrderDto if the Order exists otherwise 404NotFound</returns>
        [HttpGet("GetOrder/{orderId}")]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
        {
            try
            {
                var order = await orderRepositoryService.GetOrderById(orderId);
                return order != null ? Ok(order) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <param name="createOrderDto">CreateOrderRequestDto Object</param>
        /// <returns>Returns 200Ok response with OrderResultDto if the order is created successfully otherwise 400BadRequest with OrderResultDto</returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto createOrderDto)
        {
            try
            {
                var createOrderResult = await orderRepositoryService.CreateOrder(createOrderDto);
                return createOrderResult.Result ? Ok(createOrderResult) : BadRequest(createOrderResult);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Cancels the Order
        /// </summary>
        /// <param name="orderId">Guid</param>
        /// <returns>Returns 200Ok response with OrderResultDto if the order is cancelled successfully otherwise 400BadRequest with OrderResultDto</returns>
        [HttpPut("Cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder([FromRoute] Guid orderId)
        {
            try
            {
                var cancelRequestResult = await orderRepositoryService.CancelOrder(orderId);
                return cancelRequestResult.Result ? Ok(cancelRequestResult) : BadRequest(cancelRequestResult);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Returns the Order
        /// </summary>
        /// <param name="orderId">Guid</param>
        /// <returns>Returns 200Ok response with OrderResultDto if the order is returned successfully otherwise 400BadRequest with OrderResultDto</returns>
        [HttpPut("Return/{orderId}")]
        //This API will change the status of the order to returned
        public async Task<IActionResult> ReturnOrder([FromRoute] Guid orderId)
        {
            try
            {
                var cancelRequestResult = await orderRepositoryService.ReturnOrder(orderId);
                return cancelRequestResult.Result ? Ok(cancelRequestResult) : BadRequest(cancelRequestResult);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }
    }
}
