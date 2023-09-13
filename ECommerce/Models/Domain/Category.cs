using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class Category
{
    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public Guid? ParentCategoryId { get; set; }

    public virtual ICollection<BrandCategory> BrandCategories { get; set; } = new List<BrandCategory>();

    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    public virtual Category? ParentCategory { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<PropertyName> PropertyNames { get; set; } = new List<PropertyName>();
}
