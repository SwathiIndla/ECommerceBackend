using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class PropertyName
{
    public Guid PropertyId { get; set; }

    public Guid CategoryId { get; set; }

    public string PropertyName1 { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<PropertyValue> PropertyValues { get; set; } = new List<PropertyValue>();
}
