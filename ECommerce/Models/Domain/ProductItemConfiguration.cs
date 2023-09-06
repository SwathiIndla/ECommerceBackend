using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class ProductItemConfiguration
{
    public Guid Id { get; set; }

    public Guid PropertyValueId { get; set; }

    public Guid ProductItemId { get; set; }

    public virtual ProductItemDetail ProductItem { get; set; } = null!;

    public virtual PropertyValue PropertyValue { get; set; } = null!;
}
