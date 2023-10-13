using ECommerce.Models.DTOs;
using ECommerce.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    /// <summary>
    /// This API controller handles all the logic related to Address
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly ICustomerService customerRepositoryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerRepositoryService"></param>
        public AddressController(ICustomerService customerRepositoryService)
        {
            this.customerRepositoryService = customerRepositoryService;
        }

        /// <summary>
        /// Retrieves all the addresses of the Customer 
        /// </summary>
        /// <param name="customerId">Id of the customer</param>
        /// <returns>Returns 200Ok response with List(AddressDto) if addresses are present otherwise returns 204NoContent</returns>
        /// <response code="200">Returns List of Address of the Customer</response>
        /// <response code="204">Returns 204 No Content if no address is found or the customer is not found</response>
        /// <response code="500">Returns Internal server error with the Message when exception occurs</response>
        [HttpGet("{customerId}")]
        [ProducesResponseType(typeof(List<AddressDto>), 200)]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetAddresses([FromRoute] Guid customerId)
        {
            try
            {
                var addresses = await customerRepositoryService.GetAddressesOfCustomer(customerId);
                return addresses.Count > 0 ? Ok(addresses) : NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode( StatusCodes.Status500InternalServerError, (new { ex.Message }));
            }
        }

        /// <summary>
        /// Creates a new Address for the customer
        /// </summary>
        /// <param name="addressDto">AddAddressRequestDto object</param>
        /// <returns>If address is created successfully returns a 200OK response with AddressDto otherwise returns 400BadRequest</returns>
        /// <response code="200">Returns the New Address that is added</response>
        /// <response code="400">Returns Bad Request when the new address addition fails</response>
        /// <response code="500">Returns Internal server error with the Message when exception occurs</response>
        [HttpPost]
        [ProducesResponseType(typeof(AddressDto), 200)]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddAddress([FromBody] AddAddressRequestDto addressDto)
        {
            try
            {
                var newAddress = await customerRepositoryService.AddAddressToCustomer(addressDto);
                return newAddress != null ? Ok(newAddress) : BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (new { ex.Message }));
            }
        }

        /// <summary>
        /// Updates the already existing address of the customer
        /// </summary>
        /// <param name="updatedAddressDto">AddressDto object</param>
        /// <returns>Returns 200OK response with AddressDto if the address is found and updated successfully otherwise returns 404NotFound</returns>
        /// <response code="200">Returns the updated address details when the updation is successful</response>
        /// <response code="404">Returns Not Found status code when the address is not found</response>
        /// <response code="500">Returns Internal server error with Message when an exception occurs</response>
        [HttpPut]
        [ProducesResponseType(typeof (AddressDto), 200)]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressDto updatedAddressDto)
        {
            try
            {
                var updatedAddress = await customerRepositoryService.UpdateAddress(updatedAddressDto);
                return updatedAddress != null ? Ok(updatedAddress) : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (new { ex.Message }));
            }
        }

        /// <summary>
        /// This will set the address with given addressId as the default address of the customer
        /// </summary>
        /// <param name="addressId">A Guid</param>
        /// <returns>Returns 200Ok response if the action is successful otherwise a 400BadRequest is sent</returns>
        /// <response code="200">Returns Ok response if the address is set to default successfully</response>
        /// <response code="400">Returns Bad request if the address is not set to default</response>
        /// <response code="500">Returns Internal server error with Message when an exception occurs</response>
        [HttpPut("SetDefault/{addressId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> SetDefaultAddress([FromRoute] Guid addressId)
        {
            try
            {
                var success = await customerRepositoryService.SetDefaultAddress(addressId);
                return success ? Ok() : BadRequest();
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
        /// This will delete the address with id as addressId from the customer's address List
        /// </summary>
        /// <param name="addressId">Guid</param>
        /// <returns>Returns 200OK response if deletion is successful otherwise 400BadRequest</returns>
        /// <response code="200">Returns Ok response when the address is deleted successfully</response>
        /// <response code="400">Returns Bad request when the address deletion fails</response>
        /// <response code="500">Returns Internal server error with Message when an exception occurs</response>
        [HttpDelete("{addressId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteAddress([FromRoute] Guid addressId)
        {
            try
            {
                var success = await customerRepositoryService.DeleteAddress(addressId);
                return success ? Ok() : BadRequest();
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
