using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class ShippingOrder
{
    public Guid OrderId { get; set; }

    public Guid CustomerId { get; set; }

    public string ShippingAddress { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public string OrderStatus { get; set; } = null!;

    public virtual CustomerCredential Customer { get; set; } = null!;

    public virtual ICollection<OrderedItem> OrderedItems { get; set; } = new List<OrderedItem>();
}

