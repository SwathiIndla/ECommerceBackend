using ECommerce.Models.DTOs;
using ECommerce.Services.Interface;
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
        private readonly IOrderService orderRepositoryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderRepositoryService"></param>
        public OrderController(IOrderService orderRepositoryService)
        {
            this.orderRepositoryService = orderRepositoryService;
        }

        /// <summary>
        /// Retrieves all the order information of the customer
        /// </summary>
        /// <param name="customerId">Guid</param>
        /// <returns>Returns 200Ok response with List(OrderDto) if orders are found otherwise returns 404NotFound</returns>
        /// <response code="200">Returns List of OrderDto when there are orders present on customerId</response>
        /// <response code="404">Returns Not found when there are no orders present or the customerId does not exist</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
        [HttpGet("{customerId}")]
        [ProducesResponseType(typeof(List<OrderDto>), 200)]
        public async Task<IActionResult> GetOrders([FromRoute] Guid customerId)
        {
            try
            {
                var orderList = await orderRepositoryService.GetAllOrders(customerId);
                return orderList.Count > 0 ? Ok(orderList) : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (new
                {
                    ex.Message
                }));
            }
        }

        /// <summary>
        /// Retrieves the information of the specific order of the customer
        /// </summary>
        /// <param name="orderId">Guid</param>
        /// <returns>Returns 200Ok response with OrderDto if the Order exists otherwise 404NotFound</returns>
        /// <response code="200">Returns OrderDto when successfully found</response>
        /// <response code="404">Returns Not found when the order with given id is not found</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
        [HttpGet("GetOrder/{orderId}")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
        {
            try
            {
                var order = await orderRepositoryService.GetOrderById(orderId);
                return order != null ? Ok(order) : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (new
                {
                    ex.Message
                }));
            }
        }

        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <param name="createOrderDto">CreateOrderRequestDto Object</param>
        /// <returns>Returns 200Ok response with OrderResultDto if the order is created successfully otherwise 400BadRequest with OrderResultDto</returns>
        /// <response code="200">Returns OrderResultDto when order created successfully</response>
        /// <response code="400">Returns OrderResultDto when order creation fails or returns Exception message when an exception occurs</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
        [HttpPost]
        [ProducesResponseType(typeof(OrderResultDto), 200)]
        [ProducesErrorResponseType(typeof(OrderResultDto))]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto createOrderDto)
        {
            try
            {
                var createOrderResult = await orderRepositoryService.CreateOrder(createOrderDto);
                return createOrderResult.Result ? Ok(createOrderResult) : BadRequest(createOrderResult);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (new
                {
                    ex.Message
                }));
            }
        }

        /// <summary>
        /// Cancels the Order
        /// </summary>
        /// <param name="orderId">Guid</param>
        /// <returns>Returns 200Ok response with OrderResultDto if the order is cancelled successfully otherwise 400BadRequest with OrderResultDto</returns>
        /// <response code="200">Returns OrderResultDto when order created successfully</response>
        /// <response code="400">Returns OrderResultDto when order creation fails</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
        [HttpPut("Cancel/{orderId}")]
        [ProducesResponseType(typeof(OrderResultDto), 200)]
        [ProducesErrorResponseType(typeof(OrderResultDto))]
        public async Task<IActionResult> CancelOrder([FromRoute] Guid orderId)
        {
            try
            {
                var cancelRequestResult = await orderRepositoryService.CancelOrder(orderId);
                return cancelRequestResult.Result ? Ok(cancelRequestResult) : BadRequest(cancelRequestResult);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (new
                {
                    ex.Message
                }));
            }
        }

        /// <summary>
        /// Returns the Order
        /// </summary>
        /// <param name="orderId">Guid</param>
        /// <returns>Returns 200Ok response with OrderResultDto if the order is returned successfully otherwise 400BadRequest with OrderResultDto</returns>
        /// <response code="200">Returns OrderResultDto when order created successfully</response>
        /// <response code="400">Returns OrderResultDto when order creation fails</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
        [HttpPut("Return/{orderId}")]
        [ProducesResponseType(typeof(OrderResultDto), 200)]
        [ProducesErrorResponseType(typeof(OrderResultDto))]
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
                return StatusCode(StatusCodes.Status500InternalServerError, (new
                {
                    ex.Message
                }));
            }
        }
    }
}
