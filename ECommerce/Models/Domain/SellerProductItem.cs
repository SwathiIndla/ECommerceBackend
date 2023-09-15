using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class SellerProductItem
{
    public Guid Id { get; set; }

    public Guid SellerId { get; set; }

    public Guid ProductItemId { get; set; }

    public virtual ProductItemDetail ProductItem { get; set; } = null!;

    public virtual Seller Seller { get; set; } = null!;
}
