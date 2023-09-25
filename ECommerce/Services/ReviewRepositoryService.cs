using AutoMapper;
using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class ReviewRepositoryService : IReviewRepository
    {
        private readonly EcommerceContext dbContext;
        private readonly IMapper mapper;

        public ReviewRepositoryService(EcommerceContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<ReviewDto?> AddReview(AddReviewRequestDto addReviewDto)
        {
            ReviewDto? reviewDto = null;
            var product = await dbContext.Products.FirstOrDefaultAsync(prod => prod.ProductId == addReviewDto.ProductId);
            var customer  = await dbContext.CustomerCredentials.FirstOrDefaultAsync(cust => cust.CustomerId == addReviewDto.CustomerId);
            if(product != null && customer != null)
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
            if(review != null)
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
            if(review != null)
            {
                dbContext.ProductItemReviews.Remove(review);
                await dbContext.SaveChangesAsync();
                result = true;
            }
            return result;
        }

        public async Task<List<ReviewDto>> GetAllReviews(Guid productId, bool sortOnRatingAsc)
        {
            var reviews = await dbContext.ProductItemReviews.Include(review => review.Customer).Where(review => review.ProductId == productId).OrderByDescending(review => review.Rating).ToListAsync();
            var reviewsDto = mapper.Map<List<ReviewDto>>(reviews);
            if(reviewsDto.Count > 0 && sortOnRatingAsc)
            {
                reviewsDto = reviewsDto.OrderBy(review => review.Rating).ToList();
            }
            return reviewsDto;
        }

        public async Task<bool> IsProductReviewable(Guid customerId, Guid productId)
        {
            var orders = await dbContext.ShippingOrders.Include(order => order.OrderedItems).ThenInclude(item => item.ProductItem)
                .Where(order => order.CustomerId == customerId && order.OrderedItems.Any(item => item.ProductItem.ProductId == productId)).ToListAsync();
            return orders.Count > 0;
        }
    }
}
