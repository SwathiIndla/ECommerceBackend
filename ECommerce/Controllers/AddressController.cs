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
        public async Task<IActionResult> GetAddresses([FromRoute] Guid customerId)
        {
            var addresses = await customerRepositoryService.GetAddressesOfCustomer(customerId);
            return addresses.Count > 0 ? Ok(addresses) : NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddAddress([FromBody] AddAddressRequestDto addressDto)
        {
            var newAddress = await customerRepositoryService.AddAddressToCustomer(addressDto);
            return Ok(newAddress);
        }

        [HttpPut]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressDto updatedAddressDto)
        {
            var updatedAddress = await customerRepositoryService.UpdateAddress(updatedAddressDto);
            return updatedAddress != null ? Ok(updatedAddress) : NotFound();
        }
    }
}
