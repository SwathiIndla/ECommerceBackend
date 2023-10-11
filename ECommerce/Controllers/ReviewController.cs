using ECommerce.Models.DTOs;
using ECommerce.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    /// <summary>
    /// This API Controller handles all the logic related to the reviews of a product
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService reviewRepositoryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reviewRepositoryService"></param>
        public ReviewController(IReviewService reviewRepositoryService)
        {
            this.reviewRepositoryService = reviewRepositoryService;
        }

        /// <summary>
        /// Creates a new review for the product
        /// </summary>
        /// <param name="reviewRequestDto">AddReviewRequestDto object</param>
        /// <returns>Returns 200Ok response with ReviewDto if the review is created successfully otherwise 400BadRequest</returns>
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] AddReviewRequestDto reviewRequestDto)
        {
            try
            {
                var result = await reviewRepositoryService.AddReview(reviewRequestDto);
                return result != null ? Ok(result) : BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Checks if a customer has already given the review for the product
        /// </summary>
        /// <param name="customerId">Guid</param>
        /// <param name="productId">Guid</param>
        /// <returns>Returns 200Ok response with an Object which includes a IsAvailable flag and review key of type ReviewDto</returns>
        [HttpGet("IsReviewPresent/{customerId}/{productId}")]
        public async Task<IActionResult> IsReviewPresent([FromRoute] Guid customerId, [FromRoute] Guid productId)
        {
            try
            {
                var result = await reviewRepositoryService.IsReviewPresent(customerId, productId);
                return Ok(new { IsAvailable = result != null, review = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Edits already existing review
        /// </summary>
        /// <param name="editReviewRequestDto">EditReviewRequestDto Object</param>
        /// <returns>Returns 200Ok response with ReviewDto if the review if updated successfully otherwise 40BadRequest</returns>
        [HttpPut]
        public async Task<IActionResult> EditReview([FromBody] EditReviewRequestDto editReviewRequestDto)
        {
            try
            {
                var reviewDto = await reviewRepositoryService.EditReview(editReviewRequestDto);
                return reviewDto != null ? Ok(reviewDto) : BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Deletes a review for the product
        /// </summary>
        /// <param name="productReviewId">Guid</param>
        /// <returns>Returns 200Ok response if review deleted successfully otherwise 400BadRequest</returns>
        [HttpDelete("{productReviewId}")]
        public async Task<IActionResult> DeleteReview([FromRoute] Guid productReviewId)
        {
            try
            {
                var result = await reviewRepositoryService.DeleteReview(productReviewId);
                return result ? Ok() : BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }
        
        /// <summary>
        /// Retrieves all the reviews available for the product
        /// </summary>
        /// <param name="productId">Guid</param>
        /// <param name="sortOnRatingAsc">bool</param>
        /// <param name="page">int</param>
        /// <returns>Returns 200Ok response with ReviewSummaryDto if the reviews are present otherwise 400BadRequest</returns>
        [HttpGet("all/{productId}")]
        public async Task<IActionResult> GetAllReviews([FromRoute] Guid productId, [FromQuery] bool sortOnRatingAsc = false, [FromQuery] int page = 1)
        {
            try
            {
                var reviewsSummary = await reviewRepositoryService.GetAllReviews(productId, sortOnRatingAsc, page);
                return reviewsSummary != null ? Ok(reviewsSummary) : BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Checks if a customer can review a product(can review only if he had purchased the product earlier)
        /// </summary>
        /// <param name="customerId">Guid</param>
        /// <param name="productId">Guid</param>
        /// <returns>Returns 200OK response if the product is reviewable otherwise 404NotFound. Returns 400BadRequest if Exception occurs</returns>
        [HttpGet("IsProductReviewable/{customerId}/{productId}")]
        public async Task<IActionResult> IsProductReviewable([FromRoute] Guid customerId, [FromRoute] Guid productId)
        {
            try
            {
                var result = await reviewRepositoryService.IsProductReviewable(customerId, productId);
                return result ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }
    }
}
