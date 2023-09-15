using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class Seller
{
    public Guid SellerId { get; set; }

    public string SellerName { get; set; } = null!;

    public virtual ICollection<SellerProductItem> SellerProductItems { get; set; } = new List<SellerProductItem>();
}
