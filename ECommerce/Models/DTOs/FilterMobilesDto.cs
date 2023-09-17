using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Models.DTOs
{
    public class FilterMobilesDto
    {
        public List<Guid>? Brands { get; set; } = null;
        public List<string>? Colour { get; set; } = null;
        public List<string>? RAM { get; set; } = null;
        public List<string>? Storage { get; set; } = null;
        public List<string>? Battery { get; set; } = null;
        public List<string>? Screen_Size { get; set; } = null;
        public List<string>? Resolution { get; set; } = null;
        public List<string>? Primary_Camera { get; set; } = null;
        public List<string>? Secondary_Camera { get; set; } = null;
        public List<string>? Processor { get; set; } = null;
        public decimal? MinPrice { get; set; } = null;
        public decimal? MaxPrice { get; set; } = null;
        public int Page { get; set; } = 1;
        public bool SortOnPrice { get; set; } = false;
        public bool SortByPriceAsc { get; set; } = true;
        public bool SortOnRating { get; set; } = true;
    }
}
