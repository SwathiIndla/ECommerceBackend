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

    public string? ProductItemImage { get; set; }

    public string ProductItemName { get; set; } = null!;

    public virtual ICollection<CartProductItem> CartProductItems { get; set; } = new List<CartProductItem>();

    public virtual ICollection<OrderedItem> OrderedItems { get; set; } = new List<OrderedItem>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ProductItemConfiguration> ProductItemConfigurations { get; set; } = new List<ProductItemConfiguration>();

    public virtual ICollection<SellerProductItem> SellerProductItems { get; set; } = new List<SellerProductItem>();
}
