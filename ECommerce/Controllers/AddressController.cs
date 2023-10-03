using ECommerce.Models.DTOs;
using ECommerce.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly ICustomerRepository customerRepositoryService;

        public AddressController(ICustomerRepository customerRepositoryService)
        {
            this.customerRepositoryService = customerRepositoryService;
        }

        [HttpGet("{customerId}")]
        [Authorize(Roles = "Customer")]
        //This will retrieve all the addresses of the Customer with customerId
        public async Task<IActionResult> GetAddresses([FromRoute] Guid customerId)
        {
            var addresses = await customerRepositoryService.GetAddressesOfCustomer(customerId);
            return addresses.Count > 0 ? Ok(addresses) : NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        //This will add a new address to the Customer needs an AddAddressRequestDto object to be sent in the body
        public async Task<IActionResult> AddAddress([FromBody] AddAddressRequestDto addressDto)
        {
            var newAddress = await customerRepositoryService.AddAddressToCustomer(addressDto);
            return newAddress != null ? Ok(newAddress) : BadRequest();
        }

        [HttpPut]
        [Authorize(Roles = "Customer")]
        //This will update the address of a customer and needs an AddressDto object to be sent in body
        public async Task<IActionResult> UpdateAddress([FromBody] AddressDto updatedAddressDto)
        {
            var updatedAddress = await customerRepositoryService.UpdateAddress(updatedAddressDto);
            return updatedAddress != null ? Ok(updatedAddress) : NotFound();
        }

        [HttpPut("SetDefault/{addressId}")]
        [Authorize(Roles = "Customer")]
        //This API will set the Address with addressId as default address of the customer
        public async Task<IActionResult> SetDefaultAddress([FromRoute] Guid addressId)
        {
            var success = await customerRepositoryService.SetDefaultAddress(addressId);
            return success ? Ok() : BadRequest();
        }

        [HttpDelete("{addressId}")]
        [Authorize(Roles = "Customer")]
        //This API will delete the Address with addressId from the Customer's address list
        public async Task<IActionResult> DeleteAddress([FromRoute] Guid addressId)
        {
            var success = await customerRepositoryService.DeleteAddress(addressId);
            return success ? Ok() : BadRequest();
        }
    }
}
