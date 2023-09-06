using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class OrderType
{
    public Guid OrderTypeId { get; set; }

    public string OrderType1 { get; set; } = null!;

    public virtual ICollection<ShippingOrder> ShippingOrders { get; set; } = new List<ShippingOrder>();
}
