using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repository.Implementation
{
    public class SellerRepository : ISellerRepository
    {
        private readonly EcommerceContext dbContext;

        public SellerRepository(EcommerceContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Seller?> GetSellerAsync(Guid sellerId)
        {
            return await dbContext.Sellers.FirstOrDefaultAsync(seller => seller.SellerId == sellerId);
        }
    }
}
