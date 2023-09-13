using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class Product
{
    public Guid ProductId { get; set; }

    public Guid CategoryId { get; set; }

    public Guid BrandId { get; set; }

    public string ProductName { get; set; } = null!;

    public string ProductDescription { get; set; } = null!;

    public virtual Brand Brand { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ProductItemDetail> ProductItemDetails { get; set; } = new List<ProductItemDetail>();

    public virtual ICollection<ProductItemReview> ProductItemReviews { get; set; } = new List<ProductItemReview>();
}
