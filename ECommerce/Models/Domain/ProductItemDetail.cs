using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class ProductItemDetail
{
    public Guid ProductItemId { get; set; }

    public Guid ProductId { get; set; }

    public long QtyInStock { get; set; }

    public string? Sku { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<CartProductItem> CartProductItems { get; set; } = new List<CartProductItem>();

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ProductItemConfiguration> ProductItemConfigurations { get; set; } = new List<ProductItemConfiguration>();
}
