using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DbContext
{
    public class ECommerceAuthDbContext : IdentityDbContext
    {
        public ECommerceAuthDbContext(DbContextOptions<ECommerceAuthDbContext> dbOptions):base(dbOptions)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var customerRoleId = "52100cc0-a691-43d2-a257-4548e16d99c1";
            var sellerRoleId = "ea36e780-bc9d-4d23-89dd-ff8b92fcb92a";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = customerRoleId,
                    ConcurrencyStamp = customerRoleId,
                    Name = "Customer",
                    NormalizedName = "Customer".ToUpper()
                },
                new IdentityRole
                {
                    Id = sellerRoleId,
                    ConcurrencyStamp = sellerRoleId,
                    Name = "Seller",
                    NormalizedName = "Seller".ToUpper()
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
