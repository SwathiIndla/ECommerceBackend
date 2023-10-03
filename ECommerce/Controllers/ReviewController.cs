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
        //This API will add a review to the product
        public async Task<IActionResult> AddReview([FromBody] AddReviewRequestDto reviewRequestDto)
        {
            var result = await reviewRepositoryService.AddReview(reviewRequestDto);
            return result != null ? Ok(result) : BadRequest();
        }

        [HttpGet("IsReviewPresent/{customerId}/{productId}")]
        //This API will return a bool and the review if the customer has already given a review for the product
        public async Task<IActionResult> IsReviewPresent([FromRoute] Guid customerId, [FromRoute] Guid productId)
        {
            var result = await reviewRepositoryService.IsReviewPresent(customerId, productId);
            return Ok(new {IsAvailable = result != null, review = result});
        }

        [HttpPut]
        //This API will edit the review which is already given by the customer for a product
        public async Task<IActionResult> EditReview([FromBody] EditReviewRequestDto editReviewRequestDto)
        {
            var reviewDto = await reviewRepositoryService.EditReview(editReviewRequestDto);
            return reviewDto != null ? Ok(reviewDto) : BadRequest();
        }

        [HttpDelete("{productReviewId}")]
        //This API will delete the review of a product
        public async Task<IActionResult> DeleteReview([FromRoute] Guid productReviewId)
        {
            var result = await reviewRepositoryService.DeleteReview(productReviewId);
            return result ? Ok() : BadRequest();
        }

        [HttpGet("all/{productId}")]
        //This API will retreive all the reviews available for the product
        public async Task<IActionResult> GetAllReviews([FromRoute] Guid productId, [FromQuery] bool sortOnRatingAsc = false)
        {
            var reviewsList = await reviewRepositoryService.GetAllReviews(productId, sortOnRatingAsc);
            return reviewsList != null ? Ok(reviewsList) : BadRequest();
        }

        [HttpGet("IsProductReviewable/{customerId}/{productId}")]
        //This API will return 200OK response if the customer is allowed to review the product otherwise returns 400BadRequest
        public async Task<IActionResult> IsProductReviewable([FromRoute] Guid customerId, [FromRoute] Guid productId)
        {
            var result = await reviewRepositoryService.IsProductReviewable(customerId, productId);
            return result ? Ok() : BadRequest();
        }
    }
}
