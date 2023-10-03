using ECommerce.Models.DTOs;
using ECommerce.Repository;
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
        private readonly ICustomerRepository customerRepositoryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerRepositoryService"></param>
        public AddressController(ICustomerRepository customerRepositoryService)
        {
            this.customerRepositoryService = customerRepositoryService;
        }

        /// <summary>
        /// Retrieves all the addresses of the Customer 
        /// </summary>
        /// <param name="customerId">Id of the customer</param>
        /// <returns>Returns 200Ok response with List(AddressDto) if addresses are present otherwise returns 204NoContent</returns>
        [HttpGet("{customerId}")]
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
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Creates a new Address for the customer
        /// </summary>
        /// <param name="addressDto">AddAddressRequestDto object</param>
        /// <returns>If address is created successfully returns a 200OK response with AddressDto otherwise returns 400BadRequest</returns>
        [HttpPost]
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
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Updates the already existing address of the customer
        /// </summary>
        /// <param name="updatedAddressDto">AddressDto object</param>
        /// <returns>Returns 200OK response with AddressDto if the address is found and updated successfully otherwise returns 404NotFound</returns>
        [HttpPut]
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
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// This will set the address with given addressId as the default address of the customer
        /// </summary>
        /// <param name="addressId">A Guid</param>
        /// <returns>Returns 200Ok response if the action is successful otherwise a 400BadRequest is sent</returns>
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
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// This will delete the address with id as addressId from the customer's address List
        /// </summary>
        /// <param name="addressId">Guid</param>
        /// <returns>Returns 200OK response if deletion is successful otherwise 400BadRequest</returns>
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
                return BadRequest(new { ex.Message });
            }
        }
    }
}
