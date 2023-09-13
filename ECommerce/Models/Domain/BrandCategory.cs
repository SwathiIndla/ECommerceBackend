using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class BrandCategory
{
    public Guid Id { get; set; }

    public Guid BrandId { get; set; }

    public Guid CategoryId { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;
}

