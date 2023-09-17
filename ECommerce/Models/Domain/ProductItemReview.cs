using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class ProductItemReview
{
    public Guid ProductReviewId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid ProductId { get; set; }

    public string? Review { get; set; }

    public decimal Rating { get; set; }

    public virtual CustomerCredential Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
