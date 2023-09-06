using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class CategoryHierarchy
{
    public Guid Id { get; set; }

    public Guid AncestorCategoryId { get; set; }

    public Guid DescendantCategoryId { get; set; }

    public virtual Category AncestorCategory { get; set; } = null!;

    public virtual Category DescendantCategory { get; set; } = null!;
}
