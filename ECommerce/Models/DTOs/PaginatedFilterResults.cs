namespace ECommerce.Models.DTOs
{
    public class PaginatedFilterResults
    {
        public int TotalFilterResults { get; set; }
        public List<ProductItemCardDto> FilteredProductItems { get; set; } = null!;
    }
}
