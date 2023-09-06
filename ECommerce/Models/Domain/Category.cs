using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class Category
{
    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? CategoryImgUrl { get; set; }

    public virtual ICollection<CategoryHierarchy> CategoryHierarchyAncestorCategories { get; set; } = new List<CategoryHierarchy>();

    public virtual ICollection<CategoryHierarchy> CategoryHierarchyDescendantCategories { get; set; } = new List<CategoryHierarchy>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<PropertyName> PropertyNames { get; set; } = new List<PropertyName>();
}
