using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class Brand
{
    public Guid BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public virtual ICollection<BrandCategory> BrandCategories { get; set; } = new List<BrandCategory>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

