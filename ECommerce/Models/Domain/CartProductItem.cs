using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class CartProductItem
{
    public Guid CartProductItemId { get; set; }

    public Guid CartId { get; set; }

    public Guid ProductItemId { get; set; }

    public long Quantity { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual ProductItemDetail ProductItem { get; set; } = null!;
}