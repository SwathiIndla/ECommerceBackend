using ECommerce.Models.Domain;

namespace ECommerce.Repository
{
    public interface IProductRepository
    {
        IQueryable<ProductItemDetail> SearchProductItem(IQueryable<ProductItemDetail> query, string? search);
        object FetchMobileProductItems(string? search = null,
            List<Guid>? brands = null,
            List<string>? Colour = null,
            List<string>? RAM = null,
            List<string>? Storage = null,
            List<string>? Battery = null,
            List<string>? Screen_Size = null,
            List<string>? Resolution = null,
            List<string>? Primary_Camera = null,
            List<string>? Secondary_Camera = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1,
            bool sortOnPrice = false,
            bool sortByPriceAsc = false,
            bool sortOnRating = true,
            bool sortByRatingAsc = false);

    }
}
