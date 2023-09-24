using ECommerce.Models.DTOs;
using ECommerce.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository reviewRepositoryService;

        public ReviewController(IReviewRepository reviewRepositoryService)
        {
            this.reviewRepositoryService = reviewRepositoryService;
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] AddReviewRequestDto reviewRequestDto)
        {
            var result = await reviewRepositoryService.AddReview(reviewRequestDto);
            return result ? Ok() : BadRequest();
        }

        [HttpGet("{customerId}/{productId}")]
        public async Task<IActionResult> IsReviewPresent([FromRoute] Guid customerId, [FromRoute] Guid productId)
        {
            var result = await reviewRepositoryService.IsReviewPresent(customerId, productId);
            return Ok(new {IsAvailable = result != null, review = result});
        }

        [HttpPut]
        public async Task<IActionResult> EditReview([FromBody] EditReviewRequestDto editReviewRequestDto)
        {
            var reviewDto = await reviewRepositoryService.EditReview(editReviewRequestDto);
            return reviewDto != null ? Ok(reviewDto) : BadRequest();
        }

        [HttpDelete("{productReviewId}")]
        public async Task<IActionResult> DeleteReview([FromRoute] Guid productReviewId)
        {
            var result = await reviewRepositoryService.DeleteReview(productReviewId);
            return result ? Ok() : BadRequest();
        }

        [HttpGet("all/{productId}")]
        public async Task<IActionResult> GetAllReviews([FromRoute] Guid productId, [FromQuery] bool sortOnRatingAsc = true)
        {
            var reviewsList = await reviewRepositoryService.GetAllReviews(productId, sortOnRatingAsc);
            return reviewsList.Count != 0 ? Ok(reviewsList) : BadRequest();
        }
    }
}
