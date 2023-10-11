using AutoMapper;
using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.Implementation
{
    public class ReviewService : IReviewService
    {
        private readonly EcommerceContext dbContext;
        private readonly IMapper mapper;
        private readonly int pageSize = 10;

        public ReviewService(EcommerceContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<ReviewDto?> AddReview(AddReviewRequestDto addReviewDto)
        {
            ReviewDto? reviewDto = null;
            var product = await dbContext.Products.FirstOrDefaultAsync(prod => prod.ProductId == addReviewDto.ProductId);
            var customer = await dbContext.CustomerCredentials.FirstOrDefaultAsync(cust => cust.CustomerId == addReviewDto.CustomerId);
            if (product != null && customer != null)
            {
                var reviewDomain = mapper.Map<ProductItemReview>(addReviewDto);
                reviewDomain.ProductReviewId = Guid.NewGuid();
                await dbContext.ProductItemReviews.AddAsync(reviewDomain);
                await dbContext.SaveChangesAsync();
                reviewDto = mapper.Map<ReviewDto>(reviewDomain);
            }
            return reviewDto;
        }

        public async Task<ReviewDto?> EditReview(EditReviewRequestDto editReviewRequestDto)
        {
            ReviewDto? reviewDto = null;
            var review = await dbContext.ProductItemReviews.Include(review => review.Customer).FirstOrDefaultAsync(reviews => reviews.ProductReviewId == editReviewRequestDto.ProductReviewId);
            if (review != null)
            {
                review.Review = editReviewRequestDto.Review;
                review.Rating = editReviewRequestDto.Rating;
                await dbContext.SaveChangesAsync();
                reviewDto = mapper.Map<ReviewDto?>(review);
            }
            return reviewDto;
        }

        public async Task<ReviewDto> IsReviewPresent(Guid customerId, Guid productId)
        {
            var review = await dbContext.ProductItemReviews.Include(reviews => reviews.Customer).FirstOrDefaultAsync(reviews => reviews.CustomerId == customerId && reviews.ProductId == productId);
            var reviewDto = mapper.Map<ReviewDto>(review);
            return reviewDto;
        }

        public async Task<bool> DeleteReview(Guid productReviewId)
        {
            var review = await dbContext.ProductItemReviews.FirstOrDefaultAsync(reviews => reviews.ProductReviewId == productReviewId);
            var result = false;
            if (review != null)
            {
                dbContext.ProductItemReviews.Remove(review);
                await dbContext.SaveChangesAsync();
                result = true;
            }
            return result;
        }

        public async Task<ReviewSummaryDto?> GetAllReviews(Guid productId, bool sortOnRatingAsc, int page)
        {
            var reviews = await dbContext.ProductItemReviews.Include(review => review.Customer).Where(review => review.ProductId == productId && review.Review != null && review.Review != string.Empty).OrderByDescending(review => review.Rating).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var reviewsDto = mapper.Map<List<ReviewDto>>(reviews);
            var productDetail = await dbContext.Products.Include(product => product.ProductItemReviews).FirstOrDefaultAsync(product => product.ProductId == productId);
            ReviewSummaryDto? reviewsSummary = null;
            if (productDetail != null)
            {
                reviewsSummary = new ReviewSummaryDto();
                reviewsSummary.TotalRatings = productDetail.ProductItemReviews.Count > 0 ? productDetail.ProductItemReviews.Count : 0;
                reviewsSummary.TotalReviews = productDetail.ProductItemReviews.Count > 0 ? productDetail.ProductItemReviews.Count(product => product.Review != null) : 0;
                reviewsSummary.AverageRating = productDetail.ProductItemReviews.Count > 0 ? productDetail.ProductItemReviews.Average(product => product.Rating) : 0;
                reviewsSummary.FiveStarRatings = productDetail.ProductItemReviews.Count > 0 ? productDetail.ProductItemReviews.Count(product => product.Rating == 5) : 0;
                reviewsSummary.FourStarRatings = productDetail.ProductItemReviews.Count > 0 ? productDetail.ProductItemReviews.Count(product => product.Rating == 4) : 0;
                reviewsSummary.ThreeStarRatings = productDetail.ProductItemReviews.Count > 0 ? productDetail.ProductItemReviews.Count(product => product.Rating == 3) : 0;
                reviewsSummary.TwoStarRatings = productDetail.ProductItemReviews.Count > 0 ? productDetail.ProductItemReviews.Count(product => product.Rating == 2) : 0;
                reviewsSummary.OneStarRatings = productDetail.ProductItemReviews.Count > 0 ? productDetail.ProductItemReviews.Count(product => product.Rating == 1) : 0;
                reviewsSummary.Reviews = reviewsDto.Count > 0 && sortOnRatingAsc ? reviewsDto.OrderBy(review => review.Rating).ToList() : reviewsDto;
            }
            return reviewsSummary;
        }

        public async Task<bool> IsProductReviewable(Guid customerId, Guid productId)
        {
            var orders = await dbContext.ShippingOrders.Include(order => order.OrderedItems).ThenInclude(item => item.ProductItem)
                .Where(order => order.CustomerId == customerId && (order.OrderStatus == "Delivered" || order.OrderStatus == "Returned") && order.OrderedItems.Any(item => item.ProductItem.ProductId == productId)).ToListAsync();
            return orders.Count > 0;
        }
    }
}
