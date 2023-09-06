using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class Cart
{
    public Guid CartId { get; set; }

    public Guid CustomerId { get; set; }

    public virtual ICollection<CartProductItem> CartProductItems { get; set; } = new List<CartProductItem>();

    public virtual CustomerCredential Customer { get; set; } = null!;
}
