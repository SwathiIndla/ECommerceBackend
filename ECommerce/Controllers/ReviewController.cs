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
        /// <response code="200">Returns ReviewDto when review is added successfully</response>
        /// <response code="400">Returns Bad request when review is not added</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
        [HttpPost]
        [ProducesResponseType(typeof(ReviewDto), 200)]
        public async Task<IActionResult> AddReview([FromBody] AddReviewRequestDto reviewRequestDto)
        {
            try
            {
                var result = await reviewRepositoryService.AddReview(reviewRequestDto);
                return result != null ? Ok(result) : BadRequest();
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
        /// Checks if a customer has already given the review for the product
        /// </summary>
        /// <param name="customerId">Guid</param>
        /// <param name="productId">Guid</param>
        /// <returns>Returns 200Ok response with an Object which includes a IsAvailable flag and review key of type ReviewDto</returns>
        /// <response code="200">Returns an object with IsAvailable flag and a ReviewDto review property where review will be null if review is not present</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
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
                return StatusCode(StatusCodes.Status500InternalServerError, (new
                {
                    ex.Message
                }));
            }
        }

        /// <summary>
        /// Edits already existing review
        /// </summary>
        /// <param name="editReviewRequestDto">EditReviewRequestDto Object</param>
        /// <returns>Returns 200Ok response with ReviewDto if the review if updated successfully otherwise 40BadRequest</returns>
        /// <response code="200">Returns ReviewDto when review is edited successfully</response>
        /// <response code="400">Returns Bad request when review is not edited</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
        [HttpPut]
        [ProducesResponseType(typeof (ReviewDto), 200)]
        public async Task<IActionResult> EditReview([FromBody] EditReviewRequestDto editReviewRequestDto)
        {
            try
            {
                var reviewDto = await reviewRepositoryService.EditReview(editReviewRequestDto);
                return reviewDto != null ? Ok(reviewDto) : BadRequest();
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
        /// Deletes a review for the product
        /// </summary>
        /// <param name="productReviewId">Guid</param>
        /// <returns>Returns 200Ok response if review deleted successfully otherwise 400BadRequest</returns>
        /// <response code="200">Returns Ok response when review is deleted successfully</response>
        /// <response code="400">Returns Bad request when review is not deleted</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
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
                return StatusCode(StatusCodes.Status500InternalServerError, (new
                {
                    ex.Message
                }));
            }
        }

        /// <summary>
        /// Retrieves all the reviews available for the product
        /// </summary>
        /// <param name="productId">Guid</param>
        /// <param name="sortOnRatingAsc">bool</param>
        /// <param name="page">int</param>
        /// <returns>Returns 200Ok response with ReviewSummaryDto if the reviews are present otherwise 400BadRequest</returns>
        /// <response code="200">Returns ReviewSummaryDto</response>
        /// <response code="400">Returns Bad request when fetching the review summary fails</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
        [HttpGet("all/{productId}")]
        [ProducesResponseType(typeof(ReviewSummaryDto), 200)]
        public async Task<IActionResult> GetAllReviews([FromRoute] Guid productId, [FromQuery] bool sortOnRatingAsc = false, [FromQuery] int page = 1)
        {
            try
            {
                var reviewsSummary = await reviewRepositoryService.GetAllReviews(productId, sortOnRatingAsc, page);
                return reviewsSummary != null ? Ok(reviewsSummary) : BadRequest();
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
        /// Checks if a customer can review a product(can review only if he had purchased the product earlier)
        /// </summary>
        /// <param name="customerId">Guid</param>
        /// <param name="productId">Guid</param>
        /// <returns>Returns 200OK response if the product is reviewable otherwise 404NotFound. Returns 400BadRequest if Exception occurs</returns>
        /// <response code="200">Returns Ok response when product is reviewable by customer</response>
        /// <response code="404">Returns Not found when product is not reviewable by customer</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
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
                return StatusCode(StatusCodes.Status500InternalServerError, (new
                {
                    ex.Message
                }));
            }
        }
    }
}
